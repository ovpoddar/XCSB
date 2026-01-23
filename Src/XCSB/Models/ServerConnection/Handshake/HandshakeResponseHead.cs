using System.Runtime.InteropServices;

namespace Xcsb.Models.ServerConnection.Handshake;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 8)]
internal struct HandshakeResponseHead
{
    [FieldOffset(0)] public HandshakeStatus HandshakeStatus;
    [FieldOffset(0)] public HandshakeResponseHeadSuccess HandshakeResponseHeadSuccess;
    [FieldOffset(0)] public HandshakeResponseHeadFailed HandshakeResponseHeadFailed;
    [FieldOffset(0)] public HandshakeResponseHeadAuthenticate HandshakeResponseHeadAuthenticate;
}