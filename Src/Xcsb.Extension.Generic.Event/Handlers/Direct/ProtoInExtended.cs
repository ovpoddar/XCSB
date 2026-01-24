using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Infrastructure.Exceptions;
using Xcsb.Extension.Generic.Event.Response;
using Xcsb.Extension.Generic.Event.Response.Replies;
using Xcsb.Extension.Generic.Event.Response.Replies.Internals;
using Xcsb.Handlers.Direct;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Response.Contract;
using Xcsb.Response.Errors;
using Xcsb.Response.Event;

namespace Xcsb.Extension.Generic.Event.Handlers.Direct;

internal sealed class ProtoInExtended : ProtoIn
{
    internal ProtoInExtended(ProtoIn protoIn) : base(protoIn.Socket, protoIn.Configuration)
    {
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

    public bool HasEventToProcesses() =>
        !BufferEvents.IsEmpty || Socket.Available >= Unsafe.SizeOf<GenericEvent>();

    public void WaitForEventArrival()
    {
        if (!HasEventToProcesses())
            Socket.Poll(-1, SelectMode.SelectRead);
    }
}
