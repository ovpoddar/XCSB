using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct AllocColorReply : IXBaseResponse
{
    public readonly ResponseHeader<byte>ResponseHeader;
    public readonly ushort Red;
    public readonly ushort Green;
    public readonly ushort Blue;
    private readonly ushort _pad1;
    public readonly uint Pixel;

    public bool Verify(in int sequence)
    {
        return _pad1 == 0 && ResponseHeader.Length == 0;
    }
}