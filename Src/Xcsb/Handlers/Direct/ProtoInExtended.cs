using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Handlers;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Contract;
using Xcsb.Connection.Response.Errors;
using Xcsb.Response.Event;
using Xcsb.Response.Replies;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Handlers.Direct;

internal sealed class ProtoInExtended
{
    internal readonly SoccketAccesser _soccketAccesser;

    internal int Sequence
    {
        get => _soccketAccesser.ReceivedSequence;
        set => _soccketAccesser.ReceivedSequence = value;
    }

    internal ProtoInExtended(SoccketAccesser soccketAccesser)
    {
        _soccketAccesser = soccketAccesser;
    }


    public (ListFontsWithInfoReply[], GenericError?) ReceivedResponseArray(int sequence, int maxNames, int timeOut = 1000)
    {
        while (true)
        {
            if (sequence > Sequence)
            {
                if (_soccketAccesser.AvailableData == 0)
                    _soccketAccesser.PollRead(timeOut);
                FlushSocket();
                continue;
            }

            if (!_soccketAccesser.ReplyBuffer.Remove(sequence, out var reply))
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
            _ = _soccketAccesser.Received(headerBuffer);
            var packet = _soccketAccesser.ComputeResponse(ref headerBuffer).AsSpan();

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



    public void SkipErrorForSequence(int sequence, bool shouldThrow, [CallerMemberName] string name = "")
    {
        if (this._soccketAccesser.AvailableData == 0)
            _soccketAccesser.PollRead(1000);

        FlushSocket();
        if (!_soccketAccesser.ReplyBuffer.Remove(sequence, out var response))
            return;

        var error = response.AsSpan().AsStruct<GenericError>();
        if (!error.Verify(sequence))
            throw new Exception("Unexpected error found");

        if (shouldThrow)
            throw new XEventException(error, name);
    }


    public (T?, GenericError?) ReceivedResponse<T>(int sequence, int timeout = 1000) where T : unmanaged, IXReply
    {
        var (result, error) = _soccketAccesser.ReceivedResponseSpan<T>(sequence, timeout);
        return (result?.AsSpan().ToStruct<T>(), error);
    }

    //todo: update the code so only XEvent gets return;
    public GenericEvent ReceivedResponse()
    {
        if (_soccketAccesser.BufferEvents.TryDequeue(out var result))
            return result.AsSpan().AsStruct<GenericEvent>();
        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<GenericEvent>()];

        if (!_soccketAccesser.PollRead())
            return scratchBuffer.ToStruct<GenericEvent>();

        var totalRead = _soccketAccesser.Received(scratchBuffer, false);
        return totalRead == 0
            ? scratchBuffer.Make<GenericEvent, LastEvent>(new LastEvent(Sequence))
            : scratchBuffer.ToStruct<GenericEvent>();
    }

    public bool HasEventToProcesses() =>
        !_soccketAccesser.BufferEvents.IsEmpty || _soccketAccesser.AvailableData >= Unsafe.SizeOf<GenericEvent>();

    public void WaitForEventArrival()
    {
        if (!HasEventToProcesses())
            _soccketAccesser.PollRead();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void FlushSocket() =>
        _soccketAccesser.FlushSocket();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void FlushSocket(int outProtoSequence, bool shouldThrowOnError) =>
        _soccketAccesser.FlushSocket(outProtoSequence, shouldThrowOnError);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (byte[]?, GenericError?) ReceivedResponseSpan<T>(int sequence, int timeOut = 1000) where T : unmanaged, IXReply =>
        _soccketAccesser.ReceivedResponseSpan<T>(sequence, timeOut);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? GetVoidRequestResponse<T>(ResponseProto response) where T : struct =>
        _soccketAccesser.GetVoidRequestResponse<T>(response);
}
