using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct ListInstalledColormapsResponse : IXBaseResponse
{
    public readonly ResponseHeader ResponseHeader;
    public readonly ushort NumberOfColormaps;

    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Verify(in sequence) && this.ResponseHeader.Length == this.NumberOfColormaps;
    }
}