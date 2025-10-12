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
internal struct ProtoIn
{
    private readonly Socket _socket;

    internal readonly Queue<GenericEvent> bufferEvents;
    internal Dictionary<int, byte[]> replyBuffer;
    internal int Sequence { get; set; }
    internal ProtoIn(Socket socket)
    {
        _socket = socket;
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
                if (_socket.Available == 0)
                    _socket.Poll(timeOut, SelectMode.SelectRead);
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

        if (_socket.Poll(-1, SelectMode.SelectRead))
        {
            var totalRead = _socket.Receive(scratchBuffer);
            if (totalRead == 0)
                return scratchBuffer.Make<XEvent, LastEvent>(new(Sequence));
        }

        return scratchBuffer.ToStruct<XEvent>();
    }

    private void FlushSocket()
    {
        var bufferSize = Unsafe.SizeOf<XResponse>();
        Span<byte> buffer = stackalloc byte[bufferSize];
        while (_socket.Available != 0)
        {
            _socket.ReceiveExact(buffer);
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
                    ComputeResponse(ref buffer, _socket);
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
        if (this._socket.Available == 0)
            _socket.Poll(1000, SelectMode.SelectRead);

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
