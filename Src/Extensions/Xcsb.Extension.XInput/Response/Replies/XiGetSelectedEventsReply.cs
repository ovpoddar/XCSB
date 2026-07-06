using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct XiGetSelectedEventsReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public EventMask[] Masks;
}