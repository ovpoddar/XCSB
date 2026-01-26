using Xcsb.Extension.Generic.Event.Response.Contract;
using Xcsb.Extension.Generic.Event.Response.Replies.Internals;

namespace Xcsb.Extension.Generic.Event.Response.Replies;

public struct QueryKeymapReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public byte[] keys = new byte[32];

    internal unsafe QueryKeymapReply(QueryKeymapResponse response)
    {
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        new Span<byte>(response.Keys, 32)
            .CopyTo(keys);
    }
}