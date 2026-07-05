using System.Runtime.InteropServices;

namespace Xcsb.Connection.Models.Handshake;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
internal unsafe struct HandshakeResponseHeadAuthenticate
{
    public HandshakeStatus HandshakeStatus;
    private fixed byte _pad[5];
    public short AdditionalDataLength;
}