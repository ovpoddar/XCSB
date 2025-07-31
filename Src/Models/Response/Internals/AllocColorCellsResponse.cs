using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct AllocColorCellsResponse : IXBaseResponse
{
    public readonly ResponseHeader ResponseHeader;
    public readonly ushort NumberOfPixels;
    public readonly ushort NumberOfMasks;

    public bool Verify()
    {
        return this.ResponseHeader.Verify() && this.ResponseHeader.Length == this.NumberOfPixels + this.NumberOfMasks;
    }
}