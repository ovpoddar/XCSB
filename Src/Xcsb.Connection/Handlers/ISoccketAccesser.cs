using System.Collections.Concurrent;
using System.Net.Sockets;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Contract;
using Xcsb.Connection.Response.Errors;

namespace Xcsb.Connection.Handlers;

internal interface ISoccketAccesser
{
    int AvailableData { get; }
    ConcurrentQueue<byte[]> BufferEvents { get; }
    int ReceivedSequence { get; set; }
    ConcurrentDictionary<int, byte[]> ReplyBuffer { get; }
    int SendSequence { get; set; }

    byte[] ComputeResponse(ref Span<byte> buffer, bool updateSequence = true);
    void FlushSocket();
    void FlushSocket(int outProtoSequence, bool shouldThrowOnError);
    T? GetVoidRequestResponse<T>(ResponseProto response) where T : struct;
    bool PollRead(int timeout = -1);
    int Received(scoped in Span<byte> buffer, bool readAll = true);
    (byte[]?, GenericError?) ReceivedResponseSpan<T>(int sequence, int timeOut = 1000) where T : unmanaged, IXReply;
    void SendData(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags);
    void SendRequest(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags);
}