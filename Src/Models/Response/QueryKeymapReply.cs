using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

public struct QueryKeymapReply
{
    public readonly byte Reply;
    public readonly ushort Sequence;
    public byte[] keys;

    internal unsafe QueryKeymapReply(QueryKeymapResponse response)
    {
        this.Reply = response.Reply;
        this.Sequence = response.Sequence;
        this.keys = new Span<byte>(response.Keys, 32).ToArray();
    }
}