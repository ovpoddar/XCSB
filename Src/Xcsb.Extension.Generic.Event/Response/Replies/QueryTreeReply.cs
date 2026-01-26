using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Response.Contract;
using Xcsb.Extension.Generic.Event.Response.Replies.Internals;
using Xcsb.Helpers;

namespace Xcsb.Extension.Generic.Event.Response.Replies;

public readonly struct QueryTreeReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly uint Root;
    public readonly uint Parent;
    public readonly uint[] WindowChildren;

    internal QueryTreeReply(Span<byte> response)
    {
        ref readonly var context = ref response.AsStruct<QueryTreeResponse>();

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