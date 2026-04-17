using System.Diagnostics;
using System.Runtime.CompilerServices;
using Xcsb.Connection.Handlers;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Models;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models.TypeInfo;
using Xcsb.Response.Replies;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Handlers.Direct;

internal static class ProtoInExtended
{
    internal static (ListFontsWithInfoReply[], GenericError?) ReceivedResponseArray(this ISocketAccessor socketAccessor,
        int sequence, int maxNames, int timeOut = 1000)
    {
        while (true)
        {
            if (sequence > socketAccessor.SocketIn.Sequence)
            {
                if (socketAccessor.AvailableData == 0)
                    socketAccessor.PollRead(timeOut);
                socketAccessor.SocketIn.FlushSocket();
                continue;
            }

            if (!socketAccessor.SocketIn.ReplyBuffer.Remove(sequence, out var reply))
                throw new Exception("Should not happen.");

            var response = reply.Item1.AsSpan().AsStruct<ListFontsWithInfoResponse>();
            return response.Verify(sequence)
                ? (GetListFontsReply(socketAccessor.SocketIn, reply.Item1, sequence, maxNames), null)
                : ([], reply.Item1.AsSpan().ToStruct<GenericError>());
        }
    }

    private static ListFontsWithInfoReply[] GetListFontsReply(ISocketIn socketIn, Span<byte> reply, int sequence,
        int maxNames)
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
            _ = socketIn.Received(headerBuffer);
            var packet = socketIn.ComputeResponse(headerBuffer).AsSpan();

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

    internal static (T?, GenericError?) ReceivedResponse<T>(this ISocketIn socketIn, int sequence, int timeout = 1000)
        where T : unmanaged, IXReply
    {
        var (result, error) = socketIn.ReceivedResponseSpan<T>(sequence, timeout);
        return (result?.AsSpan().ToStruct<T>(), error);
    }

    internal static XEvent ReceivedResponse(this ISocketAccessor socketAccessor)
    {
        while (true)
        {
            if (socketAccessor.SocketIn.BufferEvents.TryDequeue(out var result))
                return new XEvent(result.Item1.AsSpan().ToStruct<XResponse>(), result.Item2);

            if (socketAccessor.PollRead())
                if (socketAccessor.AvailableData == 0)
                    return new XEvent(
                        new byte[32].AsSpan().ToStruct<XResponse>(),
                        new MappingDetails(XResponseType.Event, EventType.LastEvent));

            socketAccessor.SocketIn.FlushSocket();
        }
    }
}