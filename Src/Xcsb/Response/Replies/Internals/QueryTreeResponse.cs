using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Replies.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct QueryTreeResponse : IXReply
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Length;
    public readonly uint Root;
    public readonly uint Parent;
    public readonly ushort WindowChildrenLength;

    public bool Verify(in int sequence)
    {
        return (ResponseType)ResponseHeader.Reply == ResponseType.Reply &&
               Length == WindowChildrenLength;
    }
}