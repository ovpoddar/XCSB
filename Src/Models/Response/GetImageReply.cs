using System.Net.Sockets;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

public readonly struct GetImageReply
{
    public readonly byte Reply;
    public readonly byte Depth;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly uint VisualId;
    public readonly byte[] Data;
    internal GetImageReply(GetImageResponse result, Socket socket)
    {
        this.Reply = result.Reply;
        this.Depth = result.Depth;
        this.Sequence = result.Sequence;
        this.Length = result.Length;
        this.VisualId = result.VisualId;
        if (result.Reply == 0)
            this.Data = [];
        else
        {
            this.Data = new byte[result.Length];
            socket.ReceiveExact(this.Data);
        }
    }
}