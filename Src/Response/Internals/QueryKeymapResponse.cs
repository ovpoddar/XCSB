using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal unsafe struct QueryKeymapResponse : IXBaseResponse
{
    public readonly ResponseHeader ResponseHeader;
    public fixed byte Keys[24];

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Verify(in sequence) && ResponseHeader.Length == 2;
    }
}