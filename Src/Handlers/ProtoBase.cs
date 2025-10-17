using System.Buffers;
using System.Collections.Concurrent;
using System.Net.Sockets;
using Xcsb.Helpers;
using Xcsb.Response.Contract;
using Xcsb.Response.Event;

namespace Xcsb.Handlers;

internal abstract class ProtoBase
{
    internal readonly Socket Socket;
    internal readonly ConcurrentQueue<GenericEvent> BufferEvents;
    internal readonly ConcurrentDictionary<int, byte[]> ReplyBuffer;

    protected ProtoBase(Socket socket)
    {
        Socket = socket;
        BufferEvents = new ConcurrentQueue<GenericEvent>();
        ReplyBuffer = new ConcurrentDictionary<int, byte[]>();
    }

    protected ProtoBase(ProtoBase proto)
    {
        Socket = proto.Socket;
        BufferEvents = proto.BufferEvents;
        ReplyBuffer = proto.ReplyBuffer;
    }

    protected virtual void SendExact(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
    {
        var total = 0;
        while (total < buffer.Length)
        {
            var sent = Socket.Send(buffer[total..], socketFlags);
            if (sent <= 0)
                throw new SocketException();
            total += sent;
        }
    }

}