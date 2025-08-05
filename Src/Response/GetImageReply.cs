using System.Net.Sockets;
using Xcsb.Helpers;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

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
        Reply = result.ResponseHeader.Reply;
        Depth = result.ResponseHeader.Value;
        Sequence = result.ResponseHeader.Sequence;
        Length = result.ResponseHeader.Length;
        VisualId = result.VisualId;
        if (result.ResponseHeader.Reply == 0)
            Data = [];
        else
        {
            Data = new byte[result.ResponseHeader.Length];
            socket.ReceiveExact(Data);
        }
    }
}