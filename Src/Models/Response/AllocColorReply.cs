using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct AllocColorReply : IXBaseResponse
{
    public readonly ResponseHeader ResponseHeader;
    public readonly ushort Red;
    public readonly ushort Green;
    public readonly ushort Blue;
    private readonly ushort _pad1;
    public readonly uint Pixel;

    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Verify(in sequence) && this._pad1 == 0 && this.ResponseHeader.Length == 0;
    }
}