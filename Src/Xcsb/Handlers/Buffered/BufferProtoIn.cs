using Xcsb.Handlers.Direct;

namespace Xcsb.Handlers.Buffered;

internal sealed class BufferProtoIn
{
    private readonly ProtoInExtended _protoIn;

    public BufferProtoIn(ProtoInExtended protoIn)
    {
        _protoIn = protoIn;
    }

    internal void FlushSocket(int outProtoSequence, bool shouldThrowOnError)
    {
        var currentLength = _protoIn.Sequence;
        if (_protoIn._soccketAccesser.AvailableData == 0)
            _protoIn._soccketAccesser.PollRead(1000);

        _protoIn.FlushSocket(outProtoSequence, shouldThrowOnError);
        _protoIn.Sequence += currentLength;
    }

    internal void FlushSocket() =>
        _protoIn.FlushSocket();
}