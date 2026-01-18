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
    private readonly XcbClientConfiguration _configuration;

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
        _configuration = configuration;
    }

    protected virtual void SendExact(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
    {
        _configuration.OnSendRequest?.Invoke(Socket, socketFlags, buffer);
    }
}