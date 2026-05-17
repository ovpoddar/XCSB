using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Xcsb.Connection.Configuration;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Models;
using Xcsb.Connection.Models.TypeInfo;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Connection.Handlers;

internal class SocketIn : ISocketIn
{
    private readonly Socket _socket;
    private readonly ConcurrentDictionary<(byte, byte?), MappingDetails> _responseMap;
    private readonly XcsbClientConfiguration _configuration;

    public SocketIn(Socket socket, ConcurrentDictionary<(byte, byte?), MappingDetails> responseMap,
        XcsbClientConfiguration configuration)
    {
        _socket = socket;
        _responseMap = responseMap;
        _configuration = configuration;

        BufferEvents = new ConcurrentQueue<(byte[], MappingDetails)>();
        ReplyBuffer = new ConcurrentDictionary<int, (byte[], MappingDetails)>();
    }

    public ConcurrentQueue<(byte[], MappingDetails)> BufferEvents { get; }
    public ConcurrentDictionary<int, (byte[], MappingDetails)> ReplyBuffer { get; }

    public int Sequence { get; set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Received(scoped in Span<byte> buffer, bool readAll = true)
    {
        if (readAll)
        {
            _socket.ReceiveExact(buffer);
            _configuration.OnReceivedReply?.Invoke(buffer);
            return buffer.Length;
        }

        var totalRead = _socket.Receive(buffer);
        _configuration.OnReceivedReply?.Invoke(buffer);
        return totalRead;
    }

    // logic 1
    public void FlushSocket()
    {
        var bufferSize = Unsafe.SizeOf<XResponse>();
        var buffer = new byte[bufferSize];
        while (_socket.Available != 0)
        {
            var scratchBuffer = buffer.AsSpan();
            _ = Received(scratchBuffer);
            ref readonly var content = ref scratchBuffer.AsStruct<XResponse>();
            var responseType = GetResponseType(in content);
            switch (responseType.ResponseType)
            {
                case XResponseType.Error:
                    Sequence++;
                    ReplyBuffer[content.Sequence] = (buffer, responseType);
                    break;
                case XResponseType.Notify:
                    BufferEvents.Enqueue((buffer, responseType));
                    break;
                case XResponseType.Reply:
                    ReplyBuffer[content.Sequence] = (ComputeResponse(scratchBuffer), responseType);
                    break;
                case XResponseType.Event:
                    BufferEvents.Enqueue((buffer, responseType));
                    break;
                case XResponseType.Unknown:
                    BufferEvents.Enqueue((content.ReplyType == 35
                        ? throw new NotImplementedException() // ComputeResponse(ref buffer, false)
                        : buffer, responseType));
                    break;
                default:
                    throw new Exception(string.Join(", ", buffer.ToArray()));
            }
        }
    }

    // logic 2
    public void FlushSocket(int outProtoSequence, bool shouldThrowOnError)
    {
        var bufferSize = Unsafe.SizeOf<XResponse>();
        var buffer = new byte[bufferSize];
        while (_socket.Available != 0)
        {
            var scratchBuffer = buffer.AsSpan();
            _ = Received(scratchBuffer);
            ref readonly var content = ref scratchBuffer.AsStruct<XResponse>();
            var responseType = GetResponseType(in content);
            switch (responseType.ResponseType)
            {
                case XResponseType.Error:
                    Sequence++;
                    if (Sequence > outProtoSequence)
                        ReplyBuffer[content.Sequence] = (buffer, responseType);
                    else
                    {
                        if (shouldThrowOnError)
                            throw new XEventException(new GenericError(scratchBuffer.ToStruct<XResponse>(),
                                responseType.ErrorMessageAction!));
                    }

                    break;
                case XResponseType.Notify:
                    BufferEvents.Enqueue((buffer, responseType));
                    break;
                case XResponseType.Reply:
                    ReplyBuffer[content.Sequence] = (ComputeResponse(scratchBuffer), responseType);
                    break;
                case XResponseType.Event:
                    BufferEvents.Enqueue((buffer, responseType));
                    break;
                case XResponseType.Unknown:
                    BufferEvents.Enqueue((content.ReplyType == 35
                        ? throw new NotImplementedException() // ComputeResponse(ref buffer, false)
                        : buffer, responseType));
                    break;
                default:
                    throw new Exception(string.Join(", ", buffer));
            }
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async Task<int> ReceivedAsync(Memory<byte> buffer, CancellationToken token = default)
    {
        if (buffer.IsEmpty)
            return 0;

        var total = 0;
        while (total < buffer.Length)
        {
            var received = await _socket.ReceiveAsync(buffer[total..], SocketFlags.None, token)
                .ConfigureAwait(false);

            if (received == 0)
                return total == 0 ? -1 : total;

            total += received;
        }

        return total;
    }

    // logic 3
    public async Task<(Memory<byte>, GenericError?)> ReceivedResponseSpanAsync<T>(int sequence,
        CancellationToken token = default) where T : unmanaged, IXReply
    {
        if (sequence < Sequence)
        {
            if (ReplyBuffer.TryGetValue(sequence, out var result))
            {
                var response = result.Item1.AsStruct<T>();
                return response.Verify(in sequence)
                    ? (result.Item1, null)
                    : (Array.Empty<byte>(),
                        new GenericError(result.Item1.ToStruct<XResponse>(), result.Item2.ErrorMessageAction!));
            }
        }

        var bufferSize = Unsafe.SizeOf<XResponse>();
        Memory<byte> buffer = new byte[bufferSize];
        while (true)
        {
            var totalRead = await ReceivedAsync(buffer, token).ConfigureAwait(false);
            Debug.Assert(totalRead == bufferSize);
            ref readonly var content = ref buffer.AsStruct<XResponse>();
            var responseType = GetResponseType(in content);
            if (sequence == content.Sequence)
            {
                switch (responseType.ResponseType)
                {
                    case XResponseType.Error:
                    {
                        Sequence++;
                        var response = buffer.AsStruct<T>();
                        return response.Verify(in sequence)
                            ? (Array.Empty<byte>(),
                                new GenericError(buffer.Span.ToStruct<XResponse>(), responseType.ErrorMessageAction!))
                            : throw new Exception("Should not called");
                    }
                    case XResponseType.Reply:
                    {
                        var result = await ComputeResponseAsync(buffer, token: token).ConfigureAwait(false);
                        var response = result.AsStruct<T>();
                        return response.Verify(in sequence)
                            ? (result, null)
                            : throw new Exception("Should not called");
                    }
                    default:
                        break;
                }
            }

            switch (responseType.ResponseType)
            {
                case XResponseType.Error:
                    Sequence++;
                    ReplyBuffer[content.Sequence] = (buffer.Span.ToArray(), responseType);
                    break;
                case XResponseType.Notify:
                    BufferEvents.Enqueue((buffer.Span.ToArray(), responseType));
                    break;
                case XResponseType.Reply:
                    var key = content.Sequence;
                    var response = await ComputeResponseAsync(buffer, token: token).ConfigureAwait(false);
                    ReplyBuffer[key] = (response.ToArray(), responseType);
                    break;
                case XResponseType.Event:
                    BufferEvents.Enqueue((buffer.Span.ToArray(), responseType));
                    break;
                case XResponseType.Unknown:
                    BufferEvents.Enqueue((content.ReplyType == 35
                        ? throw new NotImplementedException() // ComputeResponse(ref buffer, false)
                        : buffer.Span.ToArray(), responseType));
                    break;
                default:
                    throw new Exception(string.Join(", ", buffer.ToArray()));
            }
        }
    }

    // logic 4
    public async Task<MappingDetails?> FlushAsync(Memory<byte> buffer, CancellationToken token = default)
    {
        if (buffer.IsEmpty || buffer.Length < Unsafe.SizeOf<XResponse>())
            throw new ArgumentException("Buffer is too small.", nameof(buffer));

        if (this.BufferEvents.TryDequeue(out var item))
        {
            item.Item1.CopyTo(buffer.Span);
            return item.Item2;
        }

        var bufferSize = Unsafe.SizeOf<XResponse>();
        while (true)
        {
            var totalRead = await ReceivedAsync(buffer, token).ConfigureAwait(false);
            if (totalRead == 0)
                return null;
            Debug.Assert(totalRead == bufferSize);
            ref readonly var content = ref buffer.AsStruct<XResponse>();
            var responseType = GetResponseType(in content);

            switch (responseType.ResponseType)
            {
                case XResponseType.Error:
                    Sequence++;
                    ReplyBuffer[content.Sequence] = (buffer.Span.ToArray(), responseType);
                    break;
                case XResponseType.Notify:
                    return responseType;
                case XResponseType.Reply:
                    var key = content.Sequence;
                    var response = await ComputeResponseAsync(buffer, token: token).ConfigureAwait(false);
                    ReplyBuffer[key] = (response.ToArray(), responseType);
                    break;
                case XResponseType.Event:
                    return responseType;
                case XResponseType.Unknown:
                    return (content.ReplyType == 35
                        ? throw new NotImplementedException() // ComputeResponse(ref buffer, false)
                        : responseType);
                default:
                    throw new Exception(string.Join(", ", buffer.ToArray()));
            }
        }
    }

    public byte[] ComputeResponse(Span<byte> buffer, bool updateSequence = true)
    {
        ref readonly var content = ref buffer.AsStruct<XResponse>();
        if (updateSequence && content.Sequence > Sequence)
            Sequence = content.Sequence;

        var replySize = (int)(content.Length * 4);
        if (replySize == 0)
            return buffer.ToArray();

        ReplyBuffer.TryRemove(content.Sequence, out var prior);
        var priorLen = prior.Item1?.Length ?? 0;
        var totalSize = 32 + replySize + priorLen;

        using var combined = new ArrayPoolUsing<byte>(totalSize);
        if (prior.Item1 is { } priorData)
            priorData.AsSpan().CopyTo(combined[..priorLen]);
        buffer.CopyTo(combined[priorLen..]);
        _ = Received(combined[(priorLen + 32)..(priorLen + 32 + replySize)]);
        return combined.Slice(0, totalSize).ToArray();
    }

    public async ValueTask<Memory<byte>> ComputeResponseAsync(Memory<byte> buffer, bool updateSequence = true,
        CancellationToken token = default)
    {
        ref readonly var content = ref buffer.AsStruct<XResponse>();
        if (updateSequence && content.Sequence > Sequence)
            Sequence = content.Sequence;

        var replySize = (int)(content.Length * 4);
        if (replySize == 0)
            return buffer.ToArray();

        var totalSize = 32 + replySize;
        Memory<byte> combined = new byte[totalSize];
        buffer.CopyTo(combined);
        var totalRead = await ReceivedAsync(combined[32..], token).ConfigureAwait(false);
        Debug.Assert(totalRead == combined.Length - 32);
        return combined;
    }

    public (byte[], GenericError?) ReceivedResponseSpan<T>(int sequence, int timeOut = 1000)
        where T : unmanaged, IXReply
    {
        while (true)
        {
            if (sequence > Sequence)
            {
                if (_socket.Available == 0)
                    _socket.Poll(timeOut, SelectMode.SelectRead);
                FlushSocket();
                continue;
            }

            if (!ReplyBuffer.Remove(sequence, out var reply))
                throw new Exception("Should not happen.");

            var response = reply.Item1.AsSpan().AsStruct<T>();
            return response.Verify(in sequence) && reply.Item2.ResponseType == XResponseType.Reply
                ? (reply.Item1, null)
                : (Array.Empty<byte>(),
                    new GenericError(reply.Item1.AsSpan().ToStruct<XResponse>(), reply.Item2.ErrorMessageAction!));
        }
    }

    public T? GetVoidRequestResponse<T>(ResponseProto response) where T : struct
    {
        if (Sequence < response.Id && !ReplyBuffer.ContainsKey(response.Id))
            FlushSocket();

        var hasAnyData = ReplyBuffer.Remove(response.Id, out var buffer);
        return hasAnyData
            ? buffer.Item1.AsSpan().AsStruct<T>()
            : response.HasReturn
                ? throw new InvalidOperationException()
                : null;
    }

    private MappingDetails GetResponseType(ref readonly XResponse reply)
    {
        var rawType = reply.Bytes[0];
        var detail = reply.Bytes[1];
        var type = (byte)(rawType & 0x7F);

        if (_responseMap.TryGetValue((type, detail), out var response)
            || _responseMap.TryGetValue((type, null), out response))
            return response;

        if (rawType != type)
            if (_responseMap.TryGetValue((rawType, detail), out response)
                || _responseMap.TryGetValue((rawType, null), out response))
                return response;

        return new MappingDetails(
            XResponseType.Unknown,
            UnknownResponse.Unknown(type)
        );
    }
}