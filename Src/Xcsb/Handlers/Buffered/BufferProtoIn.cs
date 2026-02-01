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
    internal readonly ProtoInExtended ProtoIn;

    public BufferProtoIn(ProtoInExtended protoIn)
    {
        ProtoIn = protoIn;
    }

    internal void FlushSocket(int outProtoSequence, bool shouldThrowOnError)
    {
        if (ProtoIn._soccketAccesser.Socket.Available == 0)
            ProtoIn._soccketAccesser.Socket.Poll(1000, SelectMode.SelectRead);

        ProtoIn.FlushSocket(outProtoSequence, shouldThrowOnError);
    }
}