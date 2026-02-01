using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Response.Contract;
using Xcsb.Connection.Response.Errors;
using Xcsb.Handlers.Direct;
using Xcsb.Infrastructure.Exceptions;
using Xcsb.Response.Contract;
using Xcsb.Response.Event;

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
        if (_protoIn._soccketAccesser.Socket.Available == 0)
            _protoIn._soccketAccesser.Socket.Poll(1000, SelectMode.SelectRead);

        _protoIn.FlushSocket(outProtoSequence, shouldThrowOnError);
        _protoIn.Sequence += currentLength;
    }

    internal void FlushSocket() =>
        _protoIn?.FlushSocket();
}