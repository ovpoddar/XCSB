using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

public struct GetMotionEventsReply
{
    public readonly byte Reply;
    public readonly ushort Sequence;
    public readonly TimeCoord[] Events;

    internal GetMotionEventsReply(GetMotionEventsResponse result, Socket socket)
    {
        this.Reply = result.ResponseHeader.Reply;
        this.Sequence = result.ResponseHeader.Sequence;
        if (result.NumberOfEvents == 0)
            this.Events = [];
        else
        {
            var requiredSize = (int)result.NumberOfEvents * 8;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer);
            this.Events = MemoryMarshal.Cast<byte, TimeCoord>(buffer[0..requiredSize]).ToArray();
        }
    }
}