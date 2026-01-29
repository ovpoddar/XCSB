using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Xcsb.Connection.Configuration;
using Xcsb.Connection.Helpers;

namespace Xcsb.Connection.Handlers;

internal sealed class SoccketAccesser
{
    internal readonly ConcurrentQueue<byte[]> BufferEvents;
    internal readonly ConcurrentDictionary<int, byte[]> ReplyBuffer;
    internal readonly XcsbClientConfiguration Configuration;
    internal readonly Socket Socket;
    internal int ReceivedSequence = 0;
    internal int SendSequence = 0;

    public SoccketAccesser(Socket socket,
        ConcurrentQueue<byte[]> bufferEvents, 
        ConcurrentDictionary<int, byte[]> replyBuffer,
        XcsbClientConfiguration configuration)
    {
        this.Socket = socket;
        this.BufferEvents = bufferEvents;
        this.ReplyBuffer = replyBuffer;
        this.Configuration = configuration;
    }

    public void SendExact(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags = SocketFlags.None)
    {
        Socket.SendExact(buffer, socketFlags);
        Configuration.OnSendRequest?.Invoke(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Received(scoped in Span<byte> buffer, bool readAll = true)
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
}