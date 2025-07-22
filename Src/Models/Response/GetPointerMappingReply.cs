using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

public readonly struct GetPointerMappingReply
{
    public readonly byte Reply;
    public readonly ushort Sequence;
    public readonly byte[] Map;

    internal GetPointerMappingReply(GetPointerMappingResponse response, Socket socket)
    {
        this.Reply = response.Reply;
        this.Sequence = response.Sequence;
        if (response.MapLength == 0)
            Map = [];
        else
        {
            using var mapBuffer = new ArrayPoolUsing<byte>((int)response.Length);
            socket.ReceiveExact(mapBuffer);
            Map = mapBuffer[0..(int)response.MapLength].ToArray();
        }
    }
}