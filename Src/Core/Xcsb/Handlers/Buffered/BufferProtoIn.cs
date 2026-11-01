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

    internal void FlushSocket(uint outProtoSequence, bool shouldThrowOnError)
    {
        if (_protoIn.AvailableData == 0)
            _protoIn.PollRead(1000);

        _protoIn.SocketIn.FlushSocket(outProtoSequence, shouldThrowOnError);
    }

    internal void FlushSocket() =>
        _protoIn.SocketIn.FlushSocket();
}