using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct ListPropertiesResponse : IXBaseResponse
{
    public readonly ResponseHeader ResponseHeader;
    public readonly ushort NumberOfProperties;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Verify(sequence) && ResponseHeader.Length == NumberOfProperties;
    }
}