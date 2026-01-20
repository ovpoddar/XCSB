using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Helpers;

namespace Xcsb.Models.Handshake;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 8)]
internal struct HandshakeResponseHead
{
    [FieldOffset(0)] public HandshakeStatus HandshakeStatus;
    [FieldOffset(0)] public HandshakeResponseHeadSuccess HandshakeResponseHeadSuccess;
    [FieldOffset(0)] public HandshakeResponseHeadFailed HandshakeResponseHeadFailed;
    [FieldOffset(0)] public HandshakeResponseHeadAuthenticate HandshakeResponseHeadAuthenticate;

    // for multipal call it will fail
    // todo: caching can be helpful but not needed
    internal readonly ReadOnlySpan<char> GetStatusMessage(Socket socket)
    {
        if (HandshakeStatus == HandshakeStatus.Success)
            return ReadOnlySpan<char>.Empty;

        int dataLength = HandshakeStatus == HandshakeStatus.Failed
            ? HandshakeResponseHeadFailed.AdditionalDataLength
            : HandshakeResponseHeadAuthenticate.AdditionalDataLength;

        if (dataLength == 0) throw new NotSupportedException();

        Span<byte> buffer = stackalloc byte[dataLength * 4];
        socket.ReceiveExact(buffer);
        return Encoding.ASCII.GetString(buffer).TrimEnd();
    }
}