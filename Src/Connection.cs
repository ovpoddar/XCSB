using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Models.Handshake;
using Xcsb.Models.Requests;

namespace Xcsb;

internal static class Connection
{
    private static readonly byte[] MAGIC_COOKIE = "MIT-MAGIC-COOKIE-1"u8.ToArray();

    private static string? _cachedAuthPath;
    private static readonly object _authPathLock = new();

    internal static (HandshakeSuccessResponseBody, Socket) TryConnect(ConnectionDetails connectionDetails, string display)
    {
        var (response, socket) = MakeHandshake(ref connectionDetails, display, [], []);
        if (response.HandshakeStatus == HandshakeStatus.Success && socket is not null)
        {
            var successBody = HandshakeSuccessResponseBody.Read(socket,
                response.HandshakeResponseHeadSuccess.AdditionalDataLength * 4);
            return (successBody, socket);
        }

        socket?.Dispose();
        var host = connectionDetails.Host;
        var dis = connectionDetails.Display;

        foreach (var (authName, authData) in GetAuthInfo(host, dis))
        {
            (response, socket) = MakeHandshake(ref connectionDetails, display, authName, authData);
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
        if (_cachedAuthPath != null)
            return _cachedAuthPath;

        lock (_authPathLock)
        {
            if (_cachedAuthPath != null)
                return _cachedAuthPath;

            var authPath = Environment.GetEnvironmentVariable("XAUTHORITY");
            if (!string.IsNullOrWhiteSpace(authPath))
            {
                _cachedAuthPath = authPath;
                return authPath;
            }

            var homePath = Environment.GetEnvironmentVariable("HOME");
            if (string.IsNullOrWhiteSpace(homePath))
                throw new InvalidOperationException("Neither XAUTHORITY nor HOME environment variables are set");

            _cachedAuthPath = Path.Join(homePath, ".Xauthority");
            return _cachedAuthPath;
        }
    }
    private static IEnumerable<(byte[] authName, byte[] authData)> GetAuthInfo(ReadOnlySpan<char> host, ReadOnlySpan<char> display)
    {
        var filePath = GetAuthFilePath();
        using var fileStream = File.OpenRead(filePath);
        while (fileStream.Position <= fileStream.Length)
        {
            var context = new XAuthority(fileStream);
            var dspy = context.GetDisplayNumber(fileStream);
            var displayName = context.GetName(fileStream);
            if (context.Family == ushort.MaxValue || context.Family == byte.MaxValue
                       && context.GetHostAddress(fileStream) == host
                       && (dspy is "" || dspy == display)
                       && displayName.SequenceEqual(MAGIC_COOKIE))
                yield return (displayName, context.GetData(fileStream));
        }
    }

    private static (HandshakeResponseHead, Socket?) MakeHandshake(ref ConnectionDetails connectionDetails,
        string display,
        Span<byte> authName,
        Span<byte> authData)
    {
        var socket = new Socket(AddressFamily.Unix, SocketType.Stream, connectionDetails.Protocol);
        socket.Connect(new UnixDomainSocketEndPoint(connectionDetails.GetSocketPath(display).ToString()));
        if (!socket.Connected)
            throw new Exception("Initialized failed");
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
            return (new HandshakeResponseHead(), null);
        }
    }
}
