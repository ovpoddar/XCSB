using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;
using Xcsb.Response.Contract;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Response.Replies;

public readonly struct GetMotionEventsReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly TimeCoord[] Events;

    internal GetMotionEventsReply(Span<byte> response)
    {
        ref readonly var context = ref response.AsStruct<GetMotionEventsResponse>();
        Reply = context.ResponseHeader.Reply;
        Sequence = context.ResponseHeader.Sequence;
        if (context.NumberOfEvents == 0)
            Events = [];
        else
        {

            var cursor = Unsafe.SizeOf<GetMotionEventsResponse>();
            var length = (int)context.NumberOfEvents * 8;
            Events = MemoryMarshal.Cast<byte, TimeCoord>(response[cursor..length]).ToArray();
        }
    }
}