using System.Buffers;
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
    private readonly ConcurrentDictionary<(byte, byte?, ushort?), MappingDetails> _responseMap;
    private readonly XcsbClientConfiguration _configuration;

    public SocketIn(Socket socket, ConcurrentDictionary<(byte, byte?, ushort?), MappingDetails> responseMap,
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

    public ArrayPool<byte> BufferPool => _configuration.BufferPool;
    
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
        while (_socket.Available != 0)
        {
            var buffer = _configuration.BufferPool.Rent(bufferSize);
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
                    ReplyBuffer[content.Sequence] = (ComputeResponse(buffer), responseType);
                    break;
                case XResponseType.Event:
                case XResponseType.Unknown:
                    BufferEvents.Enqueue((ComposeEvent(buffer), responseType));
                    break;
                default:
                    throw new Exception(string.Join(", ", buffer));
            }
        }
    }

    // logic 2
    public void FlushSocket(int outProtoSequence, bool shouldThrowOnError)
    {
        var bufferSize = Unsafe.SizeOf<XResponse>();
        while (_socket.Available != 0)
        {
            var buffer = _configuration.BufferPool.Rent(bufferSize);
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
                    ReplyBuffer[content.Sequence] = (ComputeResponse(buffer), responseType);
                    break;
                case XResponseType.Event:
                case XResponseType.Unknown:
                    BufferEvents.Enqueue((ComposeEvent(buffer), responseType));
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
    public async Task<(byte[], MappingDetails)> ReceivedResponseSpanAsync<T, InternalType>(int sequence,
        CancellationToken token = default) where T : struct, IXReply<T, InternalType> where InternalType : unmanaged,
        IVerify
    {
        if (sequence < Sequence && ReplyBuffer.TryGetValue(sequence, out var result1))
        {
                            
            ref readonly var temp = ref result1.Item1.AsStruct<InternalType>();
            if (!temp.Verify(in sequence))
                return (result1.Item1, new MappingDetails(XResponseType.Error, UnknownResponse.Unknown(result1.Item1[0])));

            return result1;
        }

        var bufferSize = Unsafe.SizeOf<XResponse>();
        while (true)
        {
            var buffer = _configuration.BufferPool.Rent(bufferSize);
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
                        return (buffer, responseType);
                    }
                    case XResponseType.Reply:
                    {
                        var result = await ComputeResponseAsync(buffer, token: token).ConfigureAwait(false);
                        return (result, responseType);
                    }
                    default:
                        break;
                }
            }

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
                    var key = content.Sequence;
                    var response = await ComputeResponseAsync(buffer, token: token).ConfigureAwait(false);
                    ReplyBuffer[key] = (response, responseType);
                    break;
                case XResponseType.Event:
                case XResponseType.Unknown:
                    var result = await ComposeEventAsync(buffer, token).ConfigureAwait(false);
                    BufferEvents.Enqueue((result, responseType));
                    break;
                default:
                    throw new Exception(string.Join(", ", buffer.ToArray()));
            }
        }
    }

    // logic 4
    public async Task<(MappingDetails?, byte[])> FlushAsync(CancellationToken token = default)
    {
        if (this.BufferEvents.TryDequeue(out var item))
        {
            return (item.Item2, item.Item1);
        }

        while (true)
        {
            var tempBuffer = _configuration.BufferPool.Rent(Unsafe.SizeOf<XResponse>());
            var totalRead = await ReceivedAsync(tempBuffer, token)
                .ConfigureAwait(false);
            if (totalRead == 0)
                return (null, Array.Empty<byte>());
            ref readonly var content = ref tempBuffer.AsStruct<XResponse>();
            var responseType = GetResponseType(in content);
            switch (responseType.ResponseType)
            {
                case XResponseType.Error:
                    Sequence++;
                    ReplyBuffer[content.Sequence] = (tempBuffer, responseType);
                    break;
                case XResponseType.Notify:
                    return (responseType, tempBuffer);
                case XResponseType.Reply:
                    var key = content.Sequence;
                    var response = await ComputeResponseAsync(tempBuffer, token: token).ConfigureAwait(false);
                    ReplyBuffer[key] = (response.ToArray(), responseType);
                    break;
                case XResponseType.Event:
                case XResponseType.Unknown:
                    tempBuffer = await ComposeEventAsync(tempBuffer, token).ConfigureAwait(false);
                    return (responseType, tempBuffer);
                default:
                    throw new Exception(string.Join(", ", tempBuffer.ToArray()));
            }
        }
    }

    public byte[] ComposeEvent(byte[] buffer)
    {
        ref readonly var content = ref buffer.AsStruct<XResponse>();
        if (!content.ExtensionEventType.HasValue)
            return buffer;

        var replySize = content.Length * 4;
        if (replySize == 0)
            return buffer;

        var result = _configuration.BufferPool.Rent((int)replySize + 32);
        var span = result.AsSpan();
        buffer.CopyTo(span[..32]);
        _configuration.BufferPool.Return(buffer);

        _ = Received(span[32..], true);
        return result;
    }

    public async ValueTask<byte[]> ComposeEventAsync(byte[] buffer, CancellationToken token = default)
    {
        ref readonly var content = ref buffer.AsStruct<XResponse>();
        if (!content.ExtensionEventType.HasValue)
            return buffer;
        var replySize = content.Length * 4;
        if (replySize == 0)
            return buffer;

        var result = _configuration.BufferPool.Rent((int)replySize + 32);
        var span = result.AsMemory();
        buffer.CopyTo(span[..32]);
        _configuration.BufferPool.Return(buffer);

        var totalRead = await ReceivedAsync(span[32..], token).ConfigureAwait(false);
        Debug.Assert(totalRead == result.Length - 32);
        return result;
    }


    public byte[] ComputeResponse(byte[] buffer, bool updateSequence = true)
    {
        ref readonly var content = ref buffer.AsStruct<XResponse>();
        if (updateSequence && content.Sequence > Sequence)
            Sequence = content.Sequence;

        var replySize = (int)(content.Length * 4);
        if (replySize == 0)
            return buffer;

        ReplyBuffer.TryRemove(content.Sequence, out var prior);
        var priorLen = prior.Item1?.Length ?? 0;
        var totalSize = 32 + replySize + priorLen;

        var combined = _configuration.BufferPool.Rent(totalSize);
        
        if (prior.Item1 is { } priorData)
            priorData.AsSpan().CopyTo(combined.AsSpan(0, priorLen));
        buffer.AsSpan().CopyTo(combined.AsSpan(priorLen, buffer.Length));
        _configuration.BufferPool.Return(buffer);
        _ = Received(combined.AsSpan((priorLen + 32)..(priorLen + 32 + replySize)));
        return combined;
    }

    public async ValueTask<byte[]> ComputeResponseAsync(byte[] buffer, bool updateSequence = true,
        CancellationToken token = default)
    {
        ref readonly var content = ref buffer.AsStruct<XResponse>();
        if (updateSequence && content.Sequence > Sequence)
            Sequence = content.Sequence;

        var replySize = (int)(content.Length * 4);
        if (replySize == 0)
            return buffer;

        var totalSize = 32 + replySize;
        var combined = _configuration.BufferPool.Rent(totalSize);
        buffer.CopyTo(combined);
        _configuration.BufferPool.Return(buffer);
        var totalRead = await ReceivedAsync(combined[32..], token).ConfigureAwait(false);
        Debug.Assert(totalRead == combined.Length - 32);
        return combined;
    }

    //AllocColorReply, 
    public (byte[], MappingDetails) ReceivedResponseSpan<T, InternalType>(int sequence, int timeOut = 1000)
        where T : struct, IXReply<T, InternalType> where InternalType : unmanaged, IVerify
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


            if (!ReplyBuffer.TryRemove(sequence, out var reply))
                throw new Exception("Should not happen.");
                
            ref readonly var temp = ref reply.Item1.AsStruct<InternalType>();
            if (!temp.Verify(in sequence))
                return (reply.Item1, new MappingDetails(XResponseType.Error, UnknownResponse.Unknown(reply.Item1[0])));
            return reply;
        }
    }
    
    public T? GetVoidRequestResponse<T>(ResponseProto response) where T : struct
    {
        if (Sequence < response.Id && !ReplyBuffer.ContainsKey(response.Id))
            FlushSocket();

        var hasAnyData = ReplyBuffer.TryRemove(response.Id, out var buffer);
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
        if (reply.ExtensionEventType.HasValue)
        {
            return _responseMap.TryGetValue((rawType, detail, reply.ExtensionEventType.Value), out var response)
                ? response
                : new MappingDetails(
                    XResponseType.Unknown,
                    UnknownResponse.Unknown(rawType)
                );
        }
        else
        {
            var type = (byte)(rawType & 0x7F);

            if (_responseMap.TryGetValue((type, detail, null), out var response)
                || _responseMap.TryGetValue((type, null, null), out response))
                return response;

            if (rawType != type)
                if (_responseMap.TryGetValue((rawType, detail, null), out response)
                    || _responseMap.TryGetValue((rawType, null, null), out response))
                    return response;

            return new MappingDetails(
                XResponseType.Unknown,
                UnknownResponse.Unknown(type)
            );
        }
    }
}