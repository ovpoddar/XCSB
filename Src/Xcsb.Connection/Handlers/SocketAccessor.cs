using System.Collections.Concurrent;
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

internal sealed class SocketAccessor : ISocketAccessor
{
    private readonly XcsbClientConfiguration _configuration;
    private readonly Socket _socket;
    private readonly ConcurrentDictionary<(byte, byte?), MappingDetails> _responseMap;


    public ConcurrentQueue<(byte[], MappingDetails)> BufferEvents { get; } =
        new ConcurrentQueue<(byte[], MappingDetails)>();

    public ConcurrentDictionary<int, (byte[], MappingDetails)> ReplyBuffer { get; } =
        new ConcurrentDictionary<int, (byte[], MappingDetails)>();

    public int ReceivedSequence { get; set; }
    public int SendSequence { get; set; }

    public SocketAccessor(Socket socket, ConcurrentDictionary<(byte, byte?), MappingDetails> responseMap,
        XcsbClientConfiguration configuration)
    {
        this._socket = socket;
        this._responseMap = responseMap;
        this._configuration = configuration;
    }

    private MappingDetails GetResponseType(ref readonly XResponse reply)
    {
        if (!_responseMap.TryGetValue((reply.Bytes[0], reply.Bytes[1]), out var response))
            if (!_responseMap.TryGetValue((reply.Bytes[0], null), out response))
                return new MappingDetails(XResponseType.Unknown, UnknownResponse.Unknown(reply.Bytes[0]));

        return response;
    }


    #region Send

    public void SendData(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
    {
        _socket.SendExact(buffer, socketFlags);
        _configuration.OnSendRequest?.Invoke(buffer);
    }

    public void SendRequest(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
    {
        SendData(in buffer, socketFlags);
        SendSequence++;
    }

    #endregion

    #region received

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
                    ReceivedSequence++;
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
                    ReceivedSequence++;
                    if (ReceivedSequence > outProtoSequence)
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

    public byte[] ComputeResponse(Span<byte> buffer, bool updateSequence = true)
    {
        ref readonly var content = ref buffer.AsStruct<XResponse>();
        if (updateSequence && content.Sequence > ReceivedSequence)
            ReceivedSequence = content.Sequence;

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


    public (byte[]?, GenericError?) ReceivedResponseSpan<T>(int sequence, int timeOut = 1000)
        where T : unmanaged, IXReply
    {
        while (true)
        {
            if (sequence > ReceivedSequence)
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
                : (null, new GenericError(reply.Item1.AsSpan().ToStruct<XResponse>(), reply.Item2.ErrorMessageAction!));
        }
    }

    public T? GetVoidRequestResponse<T>(ResponseProto response) where T : struct
    {
        if (ReceivedSequence < response.Id && !ReplyBuffer.ContainsKey(response.Id))
            FlushSocket();

        var hasAnyData = ReplyBuffer.Remove(response.Id, out var buffer);
        return hasAnyData
            ? buffer.Item1.AsSpan().AsStruct<T>()
            : response.HasReturn
                ? throw new InvalidOperationException()
                : null;
    }

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool PollRead(int timeout = -1) =>
        _socket.Poll(timeout, SelectMode.SelectRead);

    public int AvailableData => _socket.Available;
}