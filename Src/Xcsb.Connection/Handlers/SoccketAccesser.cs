using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Xcsb.Connection.Configuration;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Contract;
using Xcsb.Connection.Response.Errors;

namespace Xcsb.Connection.Handlers;

internal sealed class SoccketAccesser : ISoccketAccesser
{
    private readonly XcsbClientConfiguration _configuration;
    private readonly Socket _socket;

    private readonly ConcurrentQueue<byte[]> _bufferEvents;
    private readonly ConcurrentDictionary<int, byte[]> _replyBuffer;
    private int _receivedSequence = 0;
    private int _sendSequence = 0;

    public SoccketAccesser(Socket socket,
        ConcurrentQueue<byte[]> bufferEvents,
        ConcurrentDictionary<int, byte[]> replyBuffer,
        XcsbClientConfiguration configuration)
    {
        this._socket = socket;
        this._bufferEvents = bufferEvents;
        this._replyBuffer = replyBuffer;
        this._configuration = configuration;
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
        _sendSequence++;
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
        else
        {
            var totalRead = _socket.Receive(buffer);
            _configuration.OnReceivedReply?.Invoke(buffer);
            return totalRead;
        }
    }

    public void FlushSocket()
    {
        var bufferSize = Unsafe.SizeOf<XResponse>();
        Span<byte> buffer = stackalloc byte[bufferSize];
        while (_socket.Available != 0)
        {
            _ = Received(buffer);
            ref readonly var content = ref buffer.AsStruct<XResponse>();
            switch (content.GetResponseType())
            {
                case XResponseType.Error:
                    _replyBuffer[content.Sequence] = buffer.ToArray();
                    break;
                case XResponseType.Notify:
                    _bufferEvents.Enqueue(buffer.ToArray());
                    break;
                case XResponseType.Reply:
                    _replyBuffer[content.Sequence] = ComputeResponse(ref buffer);
                    break;
                case XResponseType.Event:
                    _bufferEvents.Enqueue(buffer.ToArray());
                    break;
                case XResponseType.Unknown:
                    _bufferEvents.Enqueue(content.ReplyType == 35
                        ? throw new NotImplementedException() // ComputeResponse(ref buffer, false)
                        : buffer.ToArray());
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
            switch (content.GetResponseType())
            {
                case XResponseType.Error:
                    if (_receivedSequence > outProtoSequence)
                        _replyBuffer[content.Sequence] = buffer.ToArray();
                    else
                    {
                        if (shouldThrowOnError)
                            throw new XEventException(buffer.ToStruct<GenericError>());
                    }

                    break;
                case XResponseType.Notify:
                    _bufferEvents.Enqueue(buffer.ToArray());
                    break;
                case XResponseType.Reply:
                    _replyBuffer[content.Sequence] = ComputeResponse(ref buffer);
                    break;
                case XResponseType.Event:
                    _bufferEvents.Enqueue(buffer.ToArray());
                    break;
                case XResponseType.Unknown:
                    _bufferEvents.Enqueue(content.ReplyType == 35
                        ? throw new NotImplementedException() // ComputeResponse(ref buffer, false)
                        : buffer.ToArray());
                    break;
                default:
                    throw new Exception(string.Join(", ", buffer.ToArray()));
            }
        }
    }

    public byte[] ComputeResponse(ref Span<byte> buffer, bool updateSequence = true)
    {
        ref readonly var content = ref buffer.AsStruct<XResponse>();
        if (updateSequence && content.Sequence > _receivedSequence)
            _receivedSequence = content.Sequence;

        var replySize = (int)(content.Length * 4);
        if (replySize == 0)
            return buffer.ToArray();

        using var result = new ArrayPoolUsing<byte>(32 + replySize);
        buffer.CopyTo(result[..32]);

        _ = Received(result[32..result.Length]);

        if (!_replyBuffer.TryRemove(content.Sequence, out var response))
            return result;

        replySize = result.Length + response.Length;
        using var scratchBuffer = new ArrayPoolUsing<byte>(replySize);
        response.CopyTo(scratchBuffer);
        result[0..result.Length].CopyTo(scratchBuffer[response.Length..]);
        return scratchBuffer.Slice(0, replySize).ToArray();
    }


    public (byte[]?, GenericError?) ReceivedResponseSpan<T>(int sequence, int timeOut = 1000)
        where T : unmanaged, IXReply
    {
        while (true)
        {
            if (sequence > _receivedSequence)
            {
                if (_socket.Available == 0)
                    _socket.Poll(timeOut, SelectMode.SelectRead);
                FlushSocket();
                continue;
            }

            if (!_replyBuffer.Remove(sequence, out var reply))
                throw new Exception("Should not happen.");

            var response = reply.AsSpan().AsStruct<T>();
            return response.Verify(in sequence)
                ? (reply, null)
                : (null, reply.AsSpan().ToStruct<GenericError>());
        }
    }

    public T? GetVoidRequestResponse<T>(ResponseProto response) where T : struct
    {
        if (_receivedSequence < response.Id)
            FlushSocket();

        var hasAnyData = _replyBuffer.Remove(response.Id, out var buffer);
        return hasAnyData
            ? buffer.AsSpan().AsStruct<T>()
            : response.HasReturn
                ? throw new InvalidOperationException()
                : null;
    }

    #endregion

    public bool PollRead(int timeout = -1) =>
        _socket.Poll(timeout, SelectMode.SelectRead);

    public int AvailableData => _socket.Available;

    public ConcurrentQueue<byte[]> BufferEvents => _bufferEvents;

    public ConcurrentDictionary<int, byte[]> ReplyBuffer => _replyBuffer;

    public int ReceivedSequence { get => _receivedSequence; set => _receivedSequence = value; }
    public int SendSequence { get => _sendSequence; set => _sendSequence = value; }
}