using System.Net.Sockets;
using Xcsb.Helpers;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public readonly struct GetPropertyReply
{
    public readonly byte Reply;
    public readonly byte Format;
    public readonly ushort Sequence;
    public readonly uint Type;
    public readonly byte[] Data;

    internal GetPropertyReply(GetPropertyResponse response, Socket socket)
    {
        Reply = response.ResponseHeader.Reply;
        Format = response.ResponseHeader.Value;
        Sequence = response.ResponseHeader.Sequence;
        Type = response.Type;

        if (response.ResponseHeader.Length == 0)
            Data = [];
        else
        {
            var data = new byte[response.ResponseHeader.Length * 4];
            socket.ReceiveExact(data);
            Data = data;
        }
    }


}