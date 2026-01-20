using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Xcsb.Helpers;
using Xcsb.Response.Contract;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Response.Replies;

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