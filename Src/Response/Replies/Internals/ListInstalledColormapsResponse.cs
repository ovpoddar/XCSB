using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct ListInstalledColormapsResponse : IXBaseResponse
{
    public readonly ResponseHeader<byte>ResponseHeader;
    public readonly ushort NumberOfColormaps;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Length == NumberOfColormaps;
    }
}