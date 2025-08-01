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
        Reply = response.Reply;
        Sequence = response.Sequence;
        if (response.MapLength == 0)
            Map = [];
        else
        {
            using var mapBuffer = new ArrayPoolUsing<byte>((int)response.Length * 4);
            socket.ReceiveExact(mapBuffer);
            Map = mapBuffer[0..response.MapLength].ToArray();
        }
    }
}