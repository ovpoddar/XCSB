using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal unsafe struct QueryKeymapResponse : IXBaseResponse
{
    public readonly ResponseHeader ResponseHeader;
    public fixed byte Keys[32];

    public bool Verify()
    {
        return this.ResponseHeader.Verify() && this.ResponseHeader.Length == 2;
    }
}