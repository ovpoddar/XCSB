using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal unsafe struct QueryKeymapResponse : IXReply
{
    public readonly ResponseHeader<byte>ResponseHeader;
    public readonly uint Length;
    public fixed byte Keys[24];

    public bool Verify(in int sequence)
    {
        return this.Length == 2;
    }
}