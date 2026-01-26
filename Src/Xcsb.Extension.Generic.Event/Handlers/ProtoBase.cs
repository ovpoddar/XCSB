using System.Collections.Concurrent;
using System.Net.Sockets;
using Xcsb.Configuration;
using Xcsb.Extension.Generic.Event.Response.Event;
using Xcsb.Handlers;

namespace Xcsb.Extension.Generic.Event.Handlers;

internal abstract class ProtoBase : XcbSocketAccesser
{
    internal readonly ConcurrentQueue<GenericEvent> BufferEvents;
    internal readonly ConcurrentDictionary<int, byte[]> ReplyBuffer;

    public ProtoBase(Socket socket, XcsbClientConfiguration configuration)
        : this(socket, null, configuration)
    { }

    public ProtoBase(ProtoBase proto, XcsbClientConfiguration configuration)
        : this(proto.Socket, proto, configuration)
    { }

    private ProtoBase(Socket socket, ProtoBase? proto, XcsbClientConfiguration configuration)
        : base(socket, configuration)
    {
        BufferEvents = proto?.BufferEvents ?? new ConcurrentQueue<GenericEvent>();
        ReplyBuffer = proto?.ReplyBuffer ?? new ConcurrentDictionary<int, byte[]>();
    }

    public override void SendExact(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags = SocketFlags.None)
    {
        base.SendExact(buffer, socketFlags);
    }
}
