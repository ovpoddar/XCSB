using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Response.Contract;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public readonly struct GetImageReply
{
    public readonly ResponseType Reply;
    public readonly byte Depth;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly uint VisualId;
    public readonly byte[] Data;
    internal GetImageReply(GetImageResponse result, Socket socket)
    {
        Reply = result.ResponseHeader.Reply;
        Depth = result.ResponseHeader.GetValue();
        Sequence = result.ResponseHeader.Sequence;
        Length = result.Length;
        VisualId = result.VisualId;
        if (Length == 0)
            Data = [];
        else
        {
            Data = new byte[result.Length * 4];
            socket.ReceiveExact(Data);
        }
    }
}