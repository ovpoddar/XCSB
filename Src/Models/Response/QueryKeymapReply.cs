using System.Diagnostics;
using System.Net.Sockets;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

public struct QueryKeymapReply
{
    public readonly byte Reply;
    public readonly ushort Sequence;
    public byte[] keys;

    internal unsafe QueryKeymapReply(QueryKeymapResponse response, Socket socket)
    {
        this.Reply = response.ResponseHeader.Reply;
        this.Sequence = response.ResponseHeader.Sequence;
        this.keys = new byte[32];

        Debug.Assert(response.ResponseHeader.Length * 4 == 8);
        Span<byte> buffer = stackalloc byte[(int)(response.ResponseHeader.Length * 4)];
        socket.ReceiveExact(buffer);
        
        new Span<byte>(response.Keys, 24).CopyTo(this.keys[0..24]);
        buffer.CopyTo(this.keys[24..32]);
    }
}