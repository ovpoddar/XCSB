using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Response.Contract;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public readonly struct GetMotionEventsReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly TimeCoord[] Events;

    internal GetMotionEventsReply(GetMotionEventsResponse result, Socket socket)
    {
        Reply = result.ResponseHeader.Reply;
        Sequence = result.ResponseHeader.Sequence;
        if (result.NumberOfEvents == 0)
            Events = [];
        else
        {
            var requiredSize = (int)result.NumberOfEvents * 8;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer);
            Events = MemoryMarshal.Cast<byte, TimeCoord>(buffer[0..requiredSize]).ToArray();
        }
    }
}