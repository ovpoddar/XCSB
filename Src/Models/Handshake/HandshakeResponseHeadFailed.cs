using System.Runtime.InteropServices;

namespace Src.Models.Handshake;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
internal struct HandshakeResponseHeadFailed
{
    public HandshakeStatus HandshakeStatus;
    public byte ReasonLength;
    public short MajorVersion;
    public short MinorVersion;
    public short AdditionalDataLength;
}
