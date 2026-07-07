using System;
using Xcsb.Connection.Helpers;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct XiGetSelectedEventsReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly EventMask[] Masks;

    internal XiGetSelectedEventsReply(Span<byte> result)
    {
        ref readonly var response = ref result.AsStruct<XiGetSelectedEventsResponse>();
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        if (response.NumMasks == 0)
            Masks = Array.Empty<EventMask>();
        else
        {
            Masks = new EventMask[response.NumMasks];
            var index = 32;
            for (var i = 0; i < Masks.Length; i++)
                Masks[i] = EventMask.Parse(result[index..], ref index);
        }
    }
}