using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct AllocColorCellsResponse : IXBaseResponse
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly ushort NumberOfPixels;
    public readonly ushort NumberOfMasks;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Length == NumberOfPixels + NumberOfMasks;
    }
}