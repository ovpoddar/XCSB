using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct QueryTreeResponse : IXReply
{
    public readonly ResponseHeader<byte>ResponseHeader;
    public readonly uint Length;
    public readonly uint Root;
    public readonly uint Parent;
    public readonly ushort WindowChildrenLenght;

    public bool Verify(in int sequence)
    {
        return this.Length == WindowChildrenLenght;
    }
}