using Xcsb.Connection.Handlers;
using Xcsb.Handlers.Direct;

namespace Xcsb.Handlers.Buffered;

internal sealed class BufferProtoIn
{
    private readonly ISocketAccessor _protoIn;

    public BufferProtoIn(ISocketAccessor protoIn)
    {
        _protoIn = protoIn;
    }

    internal void FlushSocket(int outProtoSequence, bool shouldThrowOnError)
    {
        var currentLength = _protoIn.SocketIn.Sequence;
        if (_protoIn.AvailableData == 0)
            _protoIn.PollRead(1000);

        _protoIn.SocketIn.FlushSocket(outProtoSequence, shouldThrowOnError);
        _protoIn.SocketIn.Sequence += currentLength;
    }

    internal void FlushSocket() =>
        _protoIn.SocketIn.FlushSocket();
}