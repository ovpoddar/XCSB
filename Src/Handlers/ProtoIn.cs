using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Models.Infrastructure.Exceptions;
using Xcsb.Response.Contract;
using Xcsb.Response.Errors;
using Xcsb.Response.Event;

namespace Xcsb.Handlers;

// todo: want to use this https://learn.microsoft.com/en-us/dotnet/communitytoolkit/high-performance/memoryowner
// looks perfectly for this situation
internal class ProtoIn
{
    internal readonly Socket Socket;
    internal readonly Queue<GenericEvent> bufferEvents;
    internal Dictionary<int, byte[]> replyBuffer;
    internal int Sequence { get; set; }
    internal ProtoIn(Socket socket)
    {
        Socket = socket;
        Sequence = 0;
        bufferEvents = new Queue<GenericEvent>();
        replyBuffer = new Dictionary<int, byte[]>();
    }

    public (T?, GenericError?) ReceivedResponse<T>(int sequence, int timeout = 1000) where T : unmanaged, IXReply
    {
        var (result, error) = ReceivedResponseSpan<T>(sequence, timeout);
        if (result == null)
            return (null, error);
        return (result.AsSpan().AsStruct<T>(), error);
    }

    public (byte[]?, GenericError?) ReceivedResponseSpan<T>(int sequence, int timeOut = 1000) where T : unmanaged, IXReply
    {
        for (var attempts = 3; attempts > 0; attempts--)
            if (sequence > Sequence)
            {
                if (Socket.Available == 0)
                    Socket.Poll(timeOut, SelectMode.SelectRead);
                FlushSocket();
            }
            else
            {
                var reply = replyBuffer[sequence];
                replyBuffer.Remove(sequence);
                var response = reply.AsSpan().AsStruct<T>();
                return response.Verify(sequence)
                    ? (reply, null)
                    : (null, reply.AsSpan().ToStruct<GenericError>());
            }
        Debug.Assert(false);
        return (null, null);
    }

    //todo: update the code so only XEvent gets return;
    public XEvent ReceivedResponse()
    {
        if (bufferEvents.TryDequeue(out var result))
            return result.As<XEvent>();
        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<XEvent>()];

        if (!Socket.Poll(-1, SelectMode.SelectRead))
            return scratchBuffer.ToStruct<XEvent>();

        var totalRead = Socket.Receive(scratchBuffer);
        return totalRead == 0
            ? scratchBuffer.Make<XEvent, LastEvent>(new LastEvent(Sequence))
            : scratchBuffer.ToStruct<XEvent>();
    }

    private void FlushSocket()
    {
        var bufferSize = Unsafe.SizeOf<XResponse>();
        Span<byte> buffer = stackalloc byte[bufferSize];
        while (Socket.Available != 0)
        {
            Socket.ReceiveExact(buffer);
            ref var content = ref buffer.AsStruct<XResponse>();
            var responseType = content.GetResponseType();
            switch (responseType)
            {
                case XResponseType.Event:
                case XResponseType.Notify:
                case XResponseType.Unknown:
                    bufferEvents.Enqueue(content.As<GenericEvent>());
                    break;
                case XResponseType.Error:
                    replyBuffer[content.Sequence] = buffer.ToArray();
                    break;
                case XResponseType.Reply:
                    ComputeResponse(ref buffer, Socket);
                    break;
                default:
                    throw new Exception(string.Join(", ", buffer.ToArray()));
            }
        }
    }

    private void ComputeResponse(ref Span<byte> buffer, Socket socket)
    {
        ref var content = ref buffer.AsStruct<RepliesHeader>();
        if (content.Sequence > Sequence)
            Sequence = content.Sequence;

        var replySize = (int)(content.Length * 4);
        Span<byte> result = stackalloc byte[32 + replySize];
        buffer.CopyTo(result[..32]);

        socket.EnsureReadSize((int)replySize);
        socket.ReceiveExact(result[32..]);

        if (replyBuffer.ContainsKey(content.Sequence))
        {
            var response = replyBuffer[content.Sequence];
            replySize = result.Length + response.Length;
            using var scratchBuffer = new ArrayPoolUsing<byte>(replySize);
            response.CopyTo(scratchBuffer);
            result.CopyTo(scratchBuffer[response.Length..]);
            replyBuffer[content.Sequence] = scratchBuffer[..replySize].ToArray();
        }
        else
        {
            replyBuffer[content.Sequence] = result.ToArray();
        }
    }

    public void SkipErrorForSequence(int sequence, bool shouldThrow, [CallerMemberName] string name = "")
    {
        if (this.Socket.Available == 0)
            Socket.Poll(1000, SelectMode.SelectRead);

        FlushSocket();
        var isAnyError = replyBuffer.TryGetValue(sequence, out var response);
        if (!isAnyError)
            return;

        var error = response.AsSpan().AsStruct<GenericError>();
        if (error.Verify(sequence) && replyBuffer.Remove(sequence))
        {
            if (!shouldThrow)
                return;
            throw new XEventException(error, name);
        }

        Environment.Exit(-10);
    }

}
