using System.Collections.Concurrent;
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
    void FlushSocket();
    void FlushSocket(int outProtoSequence, bool shouldThrowOnError);
    
    T? GetVoidRequestResponse<T>(ResponseProto response) where T : struct;
    int Received(scoped in Span<byte> buffer, bool readAll = true);
    (byte[]?, GenericError?) ReceivedResponseSpan<T>(int sequence, int timeOut = 1000) where T : unmanaged, IXReply;
}