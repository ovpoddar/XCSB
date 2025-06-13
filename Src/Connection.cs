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
    private static byte[] MAGICCOOKIE = [77, 73, 84, 45, 77, 65, 71, 73, 67, 45, 67, 79, 79, 75, 73, 69, 45, 49];// "MIT-MAGIC-COOKIE-1";
    internal static HandshakeSuccessResponseBody TryConnect(Socket socket, ReadOnlySpan<char> host, ReadOnlySpan<char> display)
    {
        var result = MakeHandshake(socket, [], []);
        if (result.HandshakeStatus is HandshakeStatus.Authenticate or HandshakeStatus.Failed)
        {
            Debug.WriteLine($"Connection: Authenticate fail {result.GetStatusMessage(socket)}");
            var (authName, authData) = GetAuthInfo(host, display);
            result = MakeHandshake(socket, authName, authData);
        }

        if (result.HandshakeStatus is HandshakeStatus.Failed or HandshakeStatus.Authenticate)
            throw new Exception(result.GetStatusMessage(socket).ToString());

        var successResponseBody = HandshakeSuccessResponseBody.Read(socket, result.HandshakeResponseHeadSuccess.AdditionalDataLength * 4);
        return successResponseBody;
    }

    private static (byte[] authName, byte[] authData) GetAuthInfo(ReadOnlySpan<char> host,
        ReadOnlySpan<char> display)
    {
        var filePath = Environment.GetEnvironmentVariable("XAUTHORITY");
        if (string.IsNullOrWhiteSpace(filePath))
        {
            filePath = Environment.GetEnvironmentVariable("HOME");
            if (string.IsNullOrWhiteSpace(filePath))
                throw new InvalidOperationException("XAUTHORITY not set and HOME not set");
            filePath = Path.Join(filePath, ".XAuthority");
        }

        if (!File.Exists(filePath))
            return ([], []);

        using var fileStream = File.OpenRead(filePath);
        while (fileStream.Position <= fileStream.Length)
        {
            var context = new XAuthority(fileStream);
            var dspy = context.GetDisplayNumber(fileStream);
            var displayName = context.GetName(fileStream);
            if (context.Family == ushort.MaxValue
                       || context.Family == byte.MaxValue && context.GetHostAddress(fileStream) == host
                       && (dspy is "" || dspy == display)
                       && displayName.SequenceEqual(MAGICCOOKIE))
                return (displayName, context.GetData(fileStream));
        }

        throw new InvalidOperationException("Invalid XAuthority file present.");
    }

    private static HandshakeResponseHead MakeHandshake(Socket socket, Span<byte> authName, Span<byte> authData)
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
        return tempBuffer.AsStruct<HandshakeResponseHead>();
    }
}