using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct ListPropertiesResponse : IXBaseResponse
{
    public readonly ResponseHeader ResponseHeader;
    public readonly ushort NumberOfProperties;

    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Verify(sequence) && this.ResponseHeader.Length == this.NumberOfProperties;
    }
}