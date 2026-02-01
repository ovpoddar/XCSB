using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Xcsb.Connection.Configuration;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;

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

    #region Send
    public void SendData(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
    {
        Socket.SendExact(buffer, socketFlags);
        Configuration.OnSendRequest?.Invoke(buffer);
    }

    public void SendRequest(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
    {
        SendData(in buffer, socketFlags);
        SendSequence++;
    }
    #endregion

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

    public void FlushSocket()
    {
        var bufferSize = Unsafe.SizeOf<XResponseNew>();
        Span<byte> buffer = stackalloc byte[bufferSize];
        while (Socket.Available != 0)
        {
            _ = Socket.Receive(buffer);
            ref readonly var content = ref buffer.AsStruct<XResponseNew>();
            switch (content.GetResponseType())
            {
                case XResponseType.Error:
                    ReplyBuffer[content.Sequence] = buffer.ToArray();
                    break;
                case XResponseType.Notify:
                    BufferEvents.Enqueue(buffer.ToArray());
                    break;
                case XResponseType.Reply:
                    ReplyBuffer[content.Sequence] = ComputeResponse(ref buffer);
                    break;
                case XResponseType.Unknown:
                case XResponseType.Event:
                    BufferEvents.Enqueue(ComputeResponse(ref buffer));
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }

    private byte[] ComputeResponse(ref Span<byte> buffer)
    {
        ref readonly var content = ref buffer.AsStruct<XResponseNew>();
        if (content.Sequence > ReceivedSequence)
            ReceivedSequence = content.Sequence;

        var replySize = (int)(content.Length * 4);
        if (replySize == 0)
            return buffer.ToArray();

        using var result = new ArrayPoolUsing<byte>(32 + replySize);
        buffer.CopyTo(result[..32]);

        _ = Received(result[32..result.Length]);

        if (!ReplyBuffer.TryRemove(content.Sequence, out var response))
            return result;

        replySize = result.Length + response.Length;
        using var scratchBuffer = new ArrayPoolUsing<byte>(replySize);
        response.CopyTo(scratchBuffer);
        result[0..result.Length].CopyTo(scratchBuffer[response.Length..]);
        return scratchBuffer;
    }
}