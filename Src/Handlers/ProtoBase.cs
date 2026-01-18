using System.Buffers;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
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
    protected bool DisposedValue;

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
        DisposedValue = false;
    }

    protected virtual void SendExact(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
    {
        Configuration.OnSendRequest?.Invoke(Socket, socketFlags, buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int Received(scoped in Span<byte> buffer, bool readAll = true)
    {
        if (readAll)
        {
            Socket.ReceiveExact(buffer);
            Configuration.OnReceivedReply?.Invoke(buffer);
            return buffer.Length;
        }
        else
        {
            var totalRead = Socket.Receive(buffer);
            Configuration.OnReceivedReply?.Invoke(buffer);
            return totalRead;
        }
    }

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (DisposedValue)
            return;

        if (disposing)
        {
            // Socket is owned by ClientConnectionContext and also disposed by that
            // Just clear the concurrent collections to free memory
            BufferEvents.Clear();
            ReplyBuffer.Clear();
        }

        DisposedValue = true;
    }

}