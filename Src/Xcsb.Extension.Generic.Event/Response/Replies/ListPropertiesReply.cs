using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;
using Xcsb.Extension.Generic.Event.Response.Contract;
using Xcsb.Extension.Generic.Event.Response.Replies.Internals;
using Xcsb.Helpers;

namespace Xcsb.Extension.Generic.Event.Response.Replies;

public readonly struct ListPropertiesReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly ATOM[] Atoms;

    internal ListPropertiesReply(Span<byte> response)
    {
        ref readonly var context = ref response.AsStruct<ListPropertiesResponse>();
        Reply = context.ResponseHeader.Reply;
        Sequence = context.ResponseHeader.Sequence;

        if (context.NumberOfProperties == 0)
            Atoms = [];
        else
        {
            var cursor = Unsafe.SizeOf<ListPropertiesResponse>();
            var length = context.NumberOfProperties * 4;
            Atoms = MemoryMarshal.Cast<byte, ATOM>(response.Slice(cursor, length)).ToArray();
        }
    }
}