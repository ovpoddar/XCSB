using System.Net.Sockets;
using Xcsb.Helpers;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public readonly struct GetPointerMappingReply
{
    public readonly byte Reply;
    public readonly ushort Sequence;
    public readonly byte[] Map;

    internal GetPointerMappingReply(GetPointerMappingResponse response, Socket socket)
    {
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        if (response.ResponseHeader.Value == 0)
            Map = [];
        else
        {
            using var mapBuffer = new ArrayPoolUsing<byte>((int)response.ResponseHeader.Length * 4);
            socket.ReceiveExact(mapBuffer);
            Map = mapBuffer[0..response.ResponseHeader.Value].ToArray();
        }
    }
}