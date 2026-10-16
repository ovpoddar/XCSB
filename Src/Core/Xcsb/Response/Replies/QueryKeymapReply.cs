using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Response.Replies;

public struct QueryKeymapReply : IXReply<QueryKeymapReply, QueryKeymapResponse>
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public byte[] keys = new byte[32];

    internal unsafe QueryKeymapReply(Span<byte> response)
    {
        var resp = response.AsStruct<QueryKeymapResponse>();
        Reply = resp.ResponseHeader.Reply;
        Sequence = resp.ResponseHeader.Sequence;
        new Span<byte>(resp.Keys, 32)
            .CopyTo(keys);
    }

    public QueryKeymapReply FromBytes(byte[] response)
    {
        return new QueryKeymapReply(response.AsSpan());
    }
}