using System.Collections.Concurrent;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Models;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Connection.Handlers;

internal interface ISocketIn
{
    int Sequence { get; set; }

    ConcurrentQueue<(byte[], MappingDetails)> BufferEvents { get; }
    ConcurrentDictionary<int, (byte[], MappingDetails)> ReplyBuffer { get; }
    byte[] ComputeResponse(Span<byte> buffer, bool updateSequence = true);
    ValueTask<Memory<byte>> ComputeResponseAsync(Memory<byte> buffer, bool updateSequence = true,
        CancellationToken token = default);
    byte[] ComposeEvent(Span<byte> buffer);
    ValueTask<Memory<byte>> ComposeEventAsync(Memory<byte> buffer, CancellationToken token = default);
    void FlushSocket();
    void FlushSocket(int outProtoSequence, bool shouldThrowOnError);
    T? GetVoidRequestResponse<T>(ResponseProto response) where T : struct;
    int Received(scoped in Span<byte> buffer, bool readAll = true);
    Task<int> ReceivedAsync(Memory<byte> buffer, CancellationToken token = default);
    (byte[], GenericError?) ReceivedResponseSpan<T>(int sequence, int timeOut = 1000) where T : unmanaged, IXReply<T>;
    Task<(Memory<byte>, GenericError?)> ReceivedResponseSpanAsync<T>(int sequence, CancellationToken token = default) where T : unmanaged, IXReply<T>;
    Task<(MappingDetails?, byte[])> FlushAsync(CancellationToken token = default);
}