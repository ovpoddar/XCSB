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
        Reply = result.Reply;
        Depth = result.Depth;
        Sequence = result.Sequence;
        Length = result.Length;
        VisualId = result.VisualId;
        if (result.Reply == 0)
            Data = [];
        else
        {
            Data = new byte[result.Length];
            socket.ReceiveExact(Data);
        }
    }
}