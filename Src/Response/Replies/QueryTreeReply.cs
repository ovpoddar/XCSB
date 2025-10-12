using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Response.Contract;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Response.Replies;

public readonly struct QueryTreeReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly uint Root;
    public readonly uint Parent;
    public readonly uint[] WindowChildren;

    internal QueryTreeReply(Span<byte> response)
    {
        ref var context = ref response.AsStruct<QueryTreeResponse>();

        Reply = context.ResponseHeader.Reply;
        Sequence = context.ResponseHeader.Sequence;
        Root = context.Root;
        Parent = context.Parent;

        if (context.WindowChildrenLength == 0)
            WindowChildren = [];
        else
        {
            var responseSize = Unsafe.SizeOf<QueryTreeResponse>();
            WindowChildren = MemoryMarshal.Cast<byte, uint>(response[responseSize..]).ToArray();
        }
    }
}