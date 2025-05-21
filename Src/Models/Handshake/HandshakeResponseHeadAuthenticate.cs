using System.Runtime.InteropServices;

namespace Src.Models.Handshake;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
internal unsafe struct HandshakeResponseHeadAuthenticate
{
    public HandshakeStatus HandshakeStatus;
    public fixed byte Pad[5];
    public short AdditionalDataLength;
}
