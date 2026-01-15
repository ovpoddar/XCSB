using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Models.Handshake;
using Xcsb.Requests;
using Xcsb.Response.Contract;

namespace Xcsb;

internal static class Connection
{
    private static readonly byte[] MagicCookie = "MIT-MAGIC-COOKIE-1"u8.ToArray();

    private static string? _cachedAuthPath;
    private static readonly object AuthPathLock = new();

    internal static (HandshakeSuccessResponseBody, Socket) TryConnect(ConnectionDetails connectionDetails,
        string display)
    {
        ReadOnlySpan<char> error = [];
        var (response, socket) = Connect(connectionDetails, display, [], [], ref error);
        if (response is not null && socket is not null)
            return (response, socket);

        ReadOnlyMemory<char> host = connectionDetails.Host.ToArray();
        ReadOnlyMemory<char> dis = connectionDetails.Display.ToArray();

        foreach (var (authName, authData) in GetAuthInfo(host, dis))
        {
            (response, socket) = Connect(connectionDetails, display, authName, authData, ref error);
            if (response is not null && socket is not null)
                return (response, socket);
        }

        throw new UnauthorizedAccessException(error.ToString());
    }

    internal static (HandshakeSuccessResponseBody, Socket) Connect(in ConnectionDetails connectionDetails,
        string display,
        Span<byte> name,
        Span<byte> data)
    {
        ReadOnlySpan<char> error = [];
        var (response, socket) = Connect(connectionDetails, display, name, data, ref error);
        if (response is not null && socket is not null)
            return (response, socket);
        throw new UnauthorizedAccessException(error.ToString());
    }

    private static (HandshakeSuccessResponseBody?, Socket?) Connect(in ConnectionDetails connectionDetails,
        string display,
        Span<byte> name,
        Span<byte> data,
        ref ReadOnlySpan<char> errorMessage)
    {
        var (response, socket) = MakeHandshake(connectionDetails, display, name, data);
        if (response.HandshakeStatus is HandshakeStatus.Success && socket is not null)
        {
            var successBody = HandshakeSuccessResponseBody.Read(socket,
                response.HandshakeResponseHeadSuccess.AdditionalDataLength * 4);
            errorMessage = "";
            return (successBody, socket);
        }

        if (socket is not null)
            errorMessage = response.GetStatusMessage(socket);
        socket?.Dispose();
        return (null, null);
    }

    private static string GetAuthFilePath()
    {
        Thread.MemoryBarrier();
        if (_cachedAuthPath is not null)
            return _cachedAuthPath;

        lock (AuthPathLock)
        {
            if (_cachedAuthPath is not null)
                return _cachedAuthPath;

            string result;
            var authPath = Environment.GetEnvironmentVariable("XAUTHORITY");
            if (!string.IsNullOrWhiteSpace(authPath))
            {
                result = authPath;
            }
            else
            {
                var homePath = Environment.GetEnvironmentVariable("HOME");
                if (string.IsNullOrWhiteSpace(homePath))
                    throw new InvalidOperationException("Neither XAUTHORITY nor HOME environment variables are set");

                result = Path.Combine(homePath, ".Xauthority");
            }

            Thread.MemoryBarrier();
            _cachedAuthPath = result;

            return result;
        }
    }

    // TODO: move all the logic to a separate file or a separate project for less cluster
    private static IEnumerable<(byte[] authName, byte[] authData)> GetAuthInfo(ReadOnlyMemory<char> host,
        ReadOnlyMemory<char> display)
    {
        var filePath = GetAuthFilePath();
        if (!File.Exists(filePath))
            throw new UnauthorizedAccessException("Failed to connect");

        using var fileStream = File.OpenRead(filePath);
        while (fileStream.Position <= fileStream.Length)
        {
            var context = new XAuthority(fileStream);
            var dspy = context.GetDisplayNumber(fileStream);
            var displayName = context.GetName(fileStream);

            if ((host.Span is "" or " " || host.Span.SequenceEqual(context.GetHostAddress(fileStream)))
                && (dspy is "" || dspy.SequenceEqual(display.Span))
                && displayName.SequenceEqual(MagicCookie))
                yield return (displayName, context.GetData(fileStream));
        }
    }

    private static (HandshakeResponseHead, Socket?) MakeHandshake(in ConnectionDetails connectionDetails,
        string display,
        Span<byte> authName,
        Span<byte> authData)
    {
        var socket = new Socket(AddressFamily.Unix, SocketType.Stream, connectionDetails.Protocol);
        socket.Connect(new UnixDomainSocketEndPoint(connectionDetails.GetSocketPath(display).ToString()));
        if (!socket.Connected)
            throw new Exception("Error connecting to X server");
        try
        {
            var request = new HandShakeRequestType((ushort)authName.Length, (ushort)authData.Length);
            socket.Send(ref request);

            var namePaddedLength = authName.Length.AddPadding();
            var scratchBufferSize = namePaddedLength + authData.Length.AddPadding();
            if (scratchBufferSize < XcbClientConfiguration.StackAllocThreshold)
            {
                Span<byte> scratchBuffer = stackalloc byte[scratchBufferSize];
                authName.CopyTo(scratchBuffer[..]);
                authData.CopyTo(scratchBuffer[namePaddedLength..]);
                socket.SendExact(scratchBuffer);
            }
            else
            {
                using var scratchBuffer = new ArrayPoolUsing<byte>(scratchBufferSize);
                authName.CopyTo(scratchBuffer[..namePaddedLength]);
                authData.CopyTo(scratchBuffer[namePaddedLength..scratchBufferSize]);
                socket.SendExact(scratchBuffer[..scratchBufferSize]);
            }

            Span<byte> tempBuffer = stackalloc byte[Marshal.SizeOf<HandshakeResponseHead>()];
            socket.ReceiveExact(tempBuffer);
            return (tempBuffer.ToStruct<HandshakeResponseHead>(), socket);
        }
        catch (Exception)
        {
            socket.Dispose();
            return (new HandshakeResponseHead(), null);
        }
    }
}