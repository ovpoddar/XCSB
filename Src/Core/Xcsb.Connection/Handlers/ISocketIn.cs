using System.Buffers;
using System.Collections.Concurrent;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Models;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Connection.Handlers;

internal interface ISocketIn
{
    int Sequence { get; set; }
    ArrayPool<byte> BufferPool { get; }

    ConcurrentQueue<(byte[], MappingDetails)> BufferEvents { get; }
    ConcurrentDictionary<int, (byte[], MappingDetails)> ReplyBuffer { get; }
    byte[] ComputeResponse(byte[] buffer, bool updateSequence = true);
    ValueTask<byte[]> ComputeResponseAsync(byte[] buffer, bool updateSequence = true,
        CancellationToken token = default);
    byte[] ComposeEvent(byte[] buffer);
    ValueTask<byte[]> ComposeEventAsync(byte[] buffer, CancellationToken token = default);
    void FlushSocket();
    void FlushSocket(int outProtoSequence, bool shouldThrowOnError);
    T? GetVoidRequestResponse<T>(ResponseProto response) where T : struct;
    int Received(scoped in Span<byte> buffer, bool readAll = true);
    Task<int> ReceivedAsync(Memory<byte> buffer, CancellationToken token = default);
    (byte[], MappingDetails) ReceivedResponseSpan<T, InternalType>(int sequence, int timeOut = 1000) 
        where T : struct, IXReply<T, InternalType> where InternalType : unmanaged, IVerify;
    Task<(byte[], MappingDetails)> ReceivedResponseSpanAsync<T, InternalType>(int sequence, CancellationToken token = default)
        where T : struct, IXReply<T, InternalType> where InternalType : unmanaged, IVerify;
    Task<(MappingDetails?, byte[])> FlushAsync(CancellationToken token = default);
}