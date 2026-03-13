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
    private static readonly ConcurrentDictionary<(byte, byte?), MappingDetails> ResponseMap =
        new ConcurrentDictionary<(byte, byte?), MappingDetails>();


    public ConcurrentQueue<(byte[], MappingDetails)> BufferEvents { get; } = new ConcurrentQueue<(byte[], MappingDetails
        )>();//notify, event, unknown
    public ConcurrentDictionary<int, (byte[], MappingDetails)> ReplyBuffer { get; } = new ConcurrentDictionary<int, (byte[],
        MappingDetails)>();// error, reply
    public int ReceivedSequence { get; set; }
    public int SendSequence { get; set; }

    static SocketAccessor()
    {
        ResponseMap.Clear();
    }

    public SocketAccessor(Socket socket, XcsbClientConfiguration configuration)
    {
        this._socket = socket;
        this._configuration = configuration;
    }

    public void RegisterReply()
    {
        ResponseMap[(1, null)] = new MappingDetails(XResponseType.Reply, null);
    }

    public void RegisterEvent<T>(XEventType type, byte? typeValue = null) where T : unmanaged, IXEvent
    {
        var value = new MappingDetails(type == 11 ? XResponseType.Notify : XResponseType.Event, type);
        value.SetEventType<T>();
        typeValue ??= type;
        ResponseMap[(typeValue.Value, null)] = value;
    }

    public void RegisterError<T>(byte typeValue, XEventType type) where T : unmanaged, IXError
    {
        var value = new MappingDetails(XResponseType.Error, type);
        value.SetErrorType<T>();
        ResponseMap[(typeValue, type)] = value;
    }

    private MappingDetails GetResponseType(ref readonly XResponse reply)
    {
        if (!ResponseMap.TryGetValue((reply.Bytes[0], reply.Bytes[1]), out var response))
            if (!ResponseMap.TryGetValue((reply.Bytes[0], null), out response))
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
        Span<byte> buffer = stackalloc byte[bufferSize];
        while (_socket.Available != 0)
        {
            _ = Received(buffer);
            ref readonly var content = ref buffer.AsStruct<XResponse>();
            var responseType = GetResponseType(in content);
            switch (responseType.ResponseType)
            {
                case XResponseType.Error:
                    ReceivedSequence++;
                    ReplyBuffer[content.Sequence] = (buffer.ToArray(), responseType);
                    break;
                case XResponseType.Notify:
                    BufferEvents.Enqueue((buffer.ToArray(), responseType));
                    break;
                case XResponseType.Reply:
                    ReplyBuffer[content.Sequence] = (ComputeResponse(ref buffer), responseType);
                    break;
                case XResponseType.Event:
                    BufferEvents.Enqueue((buffer.ToArray(), responseType));
                    break;
                case XResponseType.Unknown:
                    BufferEvents.Enqueue((content.ReplyType == 35
                        ? throw new NotImplementedException() // ComputeResponse(ref buffer, false)
                        : buffer.ToArray(), responseType));
                    break;
                default:
                    throw new Exception(string.Join(", ", buffer.ToArray()));
            }
        }
    }

    public void FlushSocket(int outProtoSequence, bool shouldThrowOnError)
    {
        var bufferSize = Unsafe.SizeOf<XResponse>();
        Span<byte> buffer = stackalloc byte[bufferSize];
        while (_socket.Available != 0)
        {
            _ = Received(buffer);
            ref readonly var content = ref buffer.AsStruct<XResponse>();
            var responseType = GetResponseType(in content);
            switch (responseType.ResponseType)
            {
                case XResponseType.Error:
                    ReceivedSequence++;
                    if (ReceivedSequence > outProtoSequence)
                        ReplyBuffer[content.Sequence] = (buffer.ToArray(), responseType);
                    else
                    {
                        if (shouldThrowOnError)
                            throw new XEventException(new GenericError(buffer.ToStruct<XResponse>(),
                                responseType.ErrorMessageAction!));
                    }

                    break;
                case XResponseType.Notify:
                    BufferEvents.Enqueue((buffer.ToArray(), responseType));
                    break;
                case XResponseType.Reply:
                    ReplyBuffer[content.Sequence] = (ComputeResponse(ref buffer), responseType);
                    break;
                case XResponseType.Event:
                    BufferEvents.Enqueue((buffer.ToArray(), responseType));
                    break;
                case XResponseType.Unknown:
                    BufferEvents.Enqueue((content.ReplyType == 35
                        ? throw new NotImplementedException() // ComputeResponse(ref buffer, false)
                        : buffer.ToArray(), responseType));
                    break;
                default:
                    throw new Exception(string.Join(", ", buffer.ToArray()));
            }
        }
    }

    public byte[] ComputeResponse(ref Span<byte> buffer, bool updateSequence = true)
    {
        ref readonly var content = ref buffer.AsStruct<XResponse>();
        if (updateSequence && content.Sequence > ReceivedSequence)
            ReceivedSequence = content.Sequence;

        var replySize = (int)(content.Length * 4);
        if (replySize == 0)
            return buffer.ToArray();

        using var result = new ArrayPoolUsing<byte>(32 + replySize);
        buffer.CopyTo(result[..32]);

        _ = Received(result[32..result.Length]);

        if (!ReplyBuffer.TryRemove(content.Sequence, out var response))
            return result;

        replySize = result.Length + response.Item1.Length;
        using var scratchBuffer = new ArrayPoolUsing<byte>(replySize);
        response.Item1.CopyTo(scratchBuffer);
        result[0..result.Length].CopyTo(scratchBuffer[response.Item1.Length..]);
        return scratchBuffer.Slice(0, replySize).ToArray();
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
        if (ReceivedSequence < response.Id)
            FlushSocket();

        var hasAnyData = ReplyBuffer.Remove(response.Id, out var buffer);
        return hasAnyData
            ? buffer.Item1.AsSpan().AsStruct<T>()
            : response.HasReturn
                ? throw new InvalidOperationException()
                : null;
    }

    #endregion

    public bool PollRead(int timeout = -1) =>
        _socket.Poll(timeout, SelectMode.SelectRead);

    public int AvailableData => _socket.Available;
}
