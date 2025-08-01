using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

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
        return ResponseHeader.Verify(in sequence) && _pad1 == 0 &&
               ResponseHeader.Length == NumberOfPixels;
    }
}