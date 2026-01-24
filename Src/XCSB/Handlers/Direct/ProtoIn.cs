using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Configuration;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Models.Infrastructure.Exceptions;
using Xcsb.Models.Infrastructure.Response;
using Xcsb.Response.Contract;
using Xcsb.Response.Errors;
using Xcsb.Response.Event;

namespace Xcsb.Handlers.Direct;

internal class ProtoIn : ProtoBase
{
    internal int Sequence { get; set; }
    internal ProtoIn(Socket socket, XcsbClientConfiguration configuration) : base(socket, configuration)
    {
        Sequence = 0;
    }

    public (T?, GenericError?) ReceivedResponse<T>(int sequence, int timeout = 1000) where T : unmanaged, IXReply
    {
        var (result, error) = ReceivedResponseSpan<T>(sequence, timeout);
        return (result?.AsSpan().ToStruct<T>(), error);
    }

    public (byte[]?, GenericError?) ReceivedResponseSpan<T>(int sequence, int timeOut = 1000) where T : unmanaged, IXReply
    {
        while (true)
        {
            if (sequence > Sequence)
            {
                if (Socket.Available == 0)
                    Socket.Poll(timeOut, SelectMode.SelectRead);
                FlushSocket();
                continue;
            }

            if (!ReplyBuffer.Remove(sequence, out var reply))
                throw new Exception("Should not happen.");

            var response = reply.AsSpan().AsStruct<T>();
            return response.Verify(sequence)
                ? (reply, null)
                : (null, reply.AsSpan().ToStruct<GenericError>());

        }
    }

    //todo: update the code so only XEvent gets return;
    public XEvent ReceivedResponse()
    {
        if (BufferEvents.TryDequeue(out var result))
            return result.As<XEvent>();
        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<XEvent>()];

        if (!Socket.Poll(-1, SelectMode.SelectRead))
            return scratchBuffer.ToStruct<XEvent>();

        var totalRead = Received(scratchBuffer, false);
        return totalRead == 0
            ? scratchBuffer.Make<XEvent, LastEvent>(new LastEvent(Sequence))
            : scratchBuffer.ToStruct<XEvent>();
    }

    internal void FlushSocket()
    {
        var bufferSize = Unsafe.SizeOf<XResponse>();
        Span<byte> buffer = stackalloc byte[bufferSize];
        while (Socket.Available != 0)
        {
            _ = Received(buffer);
            ref readonly var content = ref buffer.AsStruct<XResponse>();
            var responseType = content.GetResponseType();
            switch (responseType)
            {
                case XResponseType.Event:
                case XResponseType.Notify:
                case XResponseType.Unknown:
                    BufferEvents.Enqueue(content.As<GenericEvent>());
                    break;
                case XResponseType.Error:
                    ReplyBuffer[content.Sequence] = buffer.ToArray();
                    break;
                case XResponseType.Reply:
                    ReplyBuffer[content.Sequence] = ComputeResponse(ref buffer);
                    break;
                default:
                    throw new Exception(string.Join(", ", buffer.ToArray()));
            }
        }
    }

    protected byte[] ComputeResponse(ref Span<byte> buffer)
    {
        ref readonly var content = ref buffer.AsStruct<RepliesHeader>();
        if (content.Sequence > Sequence)
            Sequence = content.Sequence;

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

    public void SkipErrorForSequence(int sequence, bool shouldThrow, [CallerMemberName] string name = "")
    {
        if (this.Socket.Available == 0)
            Socket.Poll(1000, SelectMode.SelectRead);

        FlushSocket();
        if (!ReplyBuffer.Remove(sequence, out var response))
            return;

        var error = response.AsSpan().AsStruct<GenericError>();
        if (!error.Verify(sequence))
            throw new Exception("Unexpected error found");

        if (shouldThrow)
            throw new XEventException(error, name);
    }

    public T? GetVoidRequestResponse<T>(ResponseProto response) where T : struct
    {
        if (Sequence < response.Id)
            FlushSocket();

        var hasAnyData = ReplyBuffer.Remove(response.Id, out var buffer);
        return hasAnyData
            ? buffer.AsSpan().AsStruct<T>()
            : response.HasReturn
                ? throw new InvalidOperationException()
                : null;
    }

    public bool HasEventToProcesses() =>
        !BufferEvents.IsEmpty || Socket.Available >= Unsafe.SizeOf<GenericEvent>();

    public void WaitForEventArrival()
    {
        if (!HasEventToProcesses())
            Socket.Poll(-1, SelectMode.SelectRead);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReceiveExact(scoped in Span<byte> buffer) =>
        Received(buffer);
}
