using System.Buffers;
using System.Collections.Concurrent;
using System.Net.Sockets;
using Xcsb.Configuration;
using Xcsb.Helpers;
using Xcsb.Response.Contract;
using Xcsb.Response.Event;

namespace Xcsb.Handlers;

internal abstract class ProtoBase
{
    internal readonly XcbClientConfiguration Configuration;
    internal readonly Socket Socket;
    internal readonly ConcurrentQueue<GenericEvent> BufferEvents;
    internal readonly ConcurrentDictionary<int, byte[]> ReplyBuffer;

    protected ProtoBase(Socket socket, XcbClientConfiguration configuration)
        : this(socket, null, configuration)
    { }

    protected ProtoBase(ProtoBase proto, XcbClientConfiguration configuration)
        : this(proto.Socket, proto, configuration)
    { }

    private ProtoBase(Socket socket, ProtoBase? proto, XcbClientConfiguration configuration)
    {
        Socket = socket;
        BufferEvents = proto?.BufferEvents ?? new ConcurrentQueue<GenericEvent>();
        ReplyBuffer = proto?.ReplyBuffer ?? new ConcurrentDictionary<int, byte[]>();
        Configuration = configuration;
    }

    protected virtual void SendExact(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
    {
        Configuration.OnSendRequest?.Invoke(Socket, socketFlags, buffer);
    }
}