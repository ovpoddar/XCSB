﻿using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Models.Handshake;
using Xcsb.Models.Requests;

namespace Xcsb;

internal static class Connection
{
    private static readonly byte[] _MagicCookie = "MIT-MAGIC-COOKIE-1"u8.ToArray();

    private static string? _cachedAuthPath;
    private static readonly object _AuthPathLock = new();

    internal static (HandshakeSuccessResponseBody, Socket) TryConnect(ConnectionDetails connectionDetails, string display)
    {
        var (response, socket) = MakeHandshake(connectionDetails, display, [], []);
        if (response.HandshakeStatus == HandshakeStatus.Success && socket is not null)
        {
            var successBody = HandshakeSuccessResponseBody.Read(socket,
                response.HandshakeResponseHeadSuccess.AdditionalDataLength * 4);
            return (successBody, socket);
        }

        socket?.Dispose();
        ReadOnlyMemory<char> host =  connectionDetails.Host.ToArray();
        ReadOnlyMemory<char> dis = connectionDetails.Display.ToArray();

        foreach (var (authName, authData) in GetAuthInfo(host, dis))
        {
            (response, socket) = MakeHandshake(connectionDetails, display, authName, authData);
            if (response.HandshakeStatus is HandshakeStatus.Success && socket is not null)
            {
                var successResponseBody = HandshakeSuccessResponseBody.Read(socket, response.HandshakeResponseHeadSuccess.AdditionalDataLength * 4);
                return (successResponseBody, socket);
            }
            socket?.Dispose();
        }
        throw new UnauthorizedAccessException("Failed to connect");
    }

    private static string GetAuthFilePath()
    {
        Thread.MemoryBarrier();
        if (_cachedAuthPath is not null)
            return _cachedAuthPath;

        lock (_AuthPathLock)
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

                result = Path.Join(homePath, ".Xauthority");
            }

            Thread.MemoryBarrier();
            _cachedAuthPath = result;

            return result;
        }
    }

    // TODO: move all the logic to a separate file or a separate project for less cluster
    private static IEnumerable<(byte[] authName, byte[] authData)> GetAuthInfo(ReadOnlyMemory<char> host, ReadOnlyMemory<char> display)
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
            if ((dspy is "" || dspy.SequenceEqual(display.Span)) && displayName.SequenceEqual(_MagicCookie))
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
            if (scratchBufferSize < GlobalSetting.StackAllocThreshold)
            {
                Span<byte> scratchBuffer = stackalloc byte[scratchBufferSize];
                authName.CopyTo(scratchBuffer[0..]);
                authData.CopyTo(scratchBuffer[namePaddedLength..]);
                socket.SendExact(scratchBuffer);
            }
            else
            {
                using var scratchBuffer = new ArrayPoolUsing<byte>(scratchBufferSize);
                authName.CopyTo(scratchBuffer[0..]);
                authData.CopyTo(scratchBuffer[namePaddedLength..]);
                socket.SendExact(scratchBuffer[..scratchBufferSize]);
            }

            Span<byte> tempBuffer = stackalloc byte[Marshal.SizeOf<HandshakeResponseHead>()];
            socket.ReceiveExact(tempBuffer);
            return (tempBuffer.AsStruct<HandshakeResponseHead>(), socket);
        }
        catch (Exception)
        {
            socket.Dispose();
            return (new HandshakeResponseHead(), null);
        }
    }
}
