using System.Diagnostics;
using System.Runtime.CompilerServices;
using Xcsb.Connection.Handlers;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Models;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models.TypeInfo;
using Xcsb.Response.Replies;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Handlers.Direct;

internal sealed class ProtoInExtended
{
    private readonly ISocketAccessor _socketAccessor;

    internal int Sequence
    {
        get => _socketAccessor.ReceivedSequence;
        set => _socketAccessor.ReceivedSequence = value;
    }

    internal ProtoInExtended(ISocketAccessor socketAccessor)
    {
        _socketAccessor = socketAccessor;
    }


    public (ListFontsWithInfoReply[], GenericError?) ReceivedResponseArray(int sequence, int maxNames, int timeOut = 1000)
    {
        while (true)
        {
            if (sequence > Sequence)
            {
                if (_socketAccessor.AvailableData == 0)
                    _socketAccessor.PollRead(timeOut);
                FlushSocket();
                continue;
            }

            if (!_socketAccessor.ReplyBuffer.Remove(sequence, out var reply))
                throw new Exception("Should not happen.");

            var response = reply.Item1.AsSpan().AsStruct<ListFontsWithInfoResponse>();
            return response.Verify(sequence)
                ? (GetListFontsReply(reply.Item1, sequence, maxNames), null)
                : ([], reply.Item1.AsSpan().ToStruct<GenericError>());
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
            _ = _socketAccessor.Received(headerBuffer);
            var packet = _socketAccessor.ComputeResponse(headerBuffer).AsSpan();

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

    public int AvailableData =>
        _socketAccessor.AvailableData;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PollRead(int timeout) =>
        _socketAccessor.PollRead(timeout);

    public void SkipErrorForSequence(int sequence, bool shouldThrow, [CallerMemberName] string name = "")
    {
        if (this._socketAccessor.AvailableData == 0)
            _socketAccessor.PollRead(1000);

        FlushSocket();
        if (!_socketAccessor.ReplyBuffer.Remove(sequence, out var response))
            return;

        if (response.Item2.ResponseType != XResponseType.Error)
            throw new Exception($"Unexpected Response Found {response.Item2.ResponseType}");
        var error = new GenericError(response.Item1.AsSpan().ToStruct<XResponse>(), response.Item2.ErrorMessageAction!);
        if (shouldThrow)
            throw new XEventException(error, name);
    }


    public (T?, GenericError?) ReceivedResponse<T>(int sequence, int timeout = 1000) where T : unmanaged, IXReply
    {
        var (result, error) = _socketAccessor.ReceivedResponseSpan<T>(sequence, timeout);
        return (result?.AsSpan().ToStruct<T>(), error);
    }

    public XEvent ReceivedResponse()
    {
        while (true)
        {
            if (_socketAccessor.BufferEvents.TryDequeue(out var result)) return new XEvent(result.Item1.ToStruct<XResponse>(), result.Item2);

            if (_socketAccessor.PollRead())
                if (_socketAccessor.AvailableData == 0)
                    return new XEvent(
                        new byte[32].ToStruct<XResponse>(),
                        new MappingDetails(XResponseType.Event, EventType.LastEvent));

            _socketAccessor.FlushSocket();
        }
    }

    public bool HasEventToProcesses() =>
        !_socketAccessor.BufferEvents.IsEmpty || _socketAccessor.AvailableData >= Unsafe.SizeOf<GenericEvent>();

    public void WaitForEventArrival()
    {
        if (!HasEventToProcesses())
            _socketAccessor.PollRead();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void FlushSocket() =>
        _socketAccessor.FlushSocket();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void FlushSocket(int outProtoSequence, bool shouldThrowOnError) =>
        _socketAccessor.FlushSocket(outProtoSequence, shouldThrowOnError);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (byte[]?, GenericError?) ReceivedResponseSpan<T>(int sequence, int timeOut = 1000) where T : unmanaged, IXReply =>
        _socketAccessor.ReceivedResponseSpan<T>(sequence, timeOut);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? GetVoidRequestResponse<T>(ResponseProto response) where T : struct =>
        _socketAccessor.GetVoidRequestResponse<T>(response);
}
