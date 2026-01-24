using Xcsb.Extension.Generic.Event.Response.Replies.Internals;
using Xcsb.Response.Contract;

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