using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Configuration;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Models.Infrastructure.Exceptions;
using Xcsb.Response.Contract;
using Xcsb.Response.Errors;
using Xcsb.Response.Event;
using Xcsb.Response.Replies;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Handlers.Direct;

internal class ProtoIn : ProtoBase
{
    internal int Sequence { get; set; }
    internal ProtoIn(Socket socket, XcbClientConfiguration configuration) : base(socket, configuration)
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

    private byte[] ComputeResponse(ref Span<byte> buffer)
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

    public (ListFontsWithInfoReply[], GenericError?) ReceivedResponseArray(int sequence, int maxNames, int timeOut = 1000)
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

            var response = reply.AsSpan().AsStruct<ListFontsWithInfoResponse>();
            return response.Verify(sequence)
                ? (GetListFontsReply(reply, sequence, maxNames), null)
                : ([], reply.AsSpan().ToStruct<GenericError>());
        }
    }

    private ListFontsWithInfoReply[] GetListFontsReply(Span<byte> reply, int sequence, int maxNames)
    {
        var result = new ArrayPoolUsing<ListFontsWithInfoReply>(maxNames);
        var count = 0;
        var cursor = 0;

        while (cursor < reply.Length)
        {
            ref readonly var response = ref reply[cursor..].AsStruct<ListFontsWithInfoResponse>();
            if (!response.HasMore) return result[0..count].ToArray();

            if (count == result.Length)
            {
                var larger = new ArrayPoolUsing<ListFontsWithInfoReply>(result.Length << 1);
                result[0..result.Length].CopyTo(larger);
                result.Dispose();
                result = larger;
            }
            cursor += Unsafe.SizeOf<ListFontsWithInfoResponse>();
            var responseLength = (int)(response.Length * 4) - 28;
            result[count++] = new ListFontsWithInfoReply(in response, reply.Slice(cursor, responseLength));
            cursor += responseLength;
        }

        Span<byte> headerBuffer = stackalloc byte[(Unsafe.SizeOf<XResponse>())];

        while (true)
        {
            _ = Received(headerBuffer);
            var packet = ComputeResponse(ref headerBuffer).AsSpan();

            ref readonly var response = ref packet.AsStruct<ListFontsWithInfoResponse>();
            Debug.Assert(response.ResponseHeader.Sequence == sequence);
            if (!response.HasMore) return result[0..count].ToArray();

            result[count++] = new ListFontsWithInfoReply(in response, packet[60..]);

            if (count != result.Length) continue;

            var larger = new ArrayPoolUsing<ListFontsWithInfoReply>(result.Length << 1);
            result[0..result.Length].CopyTo(larger);
            result.Dispose();
            result = larger;
        }

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReceiveExact(scoped in Span<byte> buffer) =>
        Received(buffer);
}
