using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct AllocColorPlanesResponse : IXBaseResponse
{
    public readonly ResponseHeader ResponseHeader;
    public readonly ushort NumberOfPixels;
    private readonly ushort _pad1;
    public readonly uint RedMask;
    public readonly uint GreenMask;
    public readonly uint BlueMask;

    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Verify(in sequence) && this._pad1 == 0 &&
               this.ResponseHeader.Length == this.NumberOfPixels;
    }
}