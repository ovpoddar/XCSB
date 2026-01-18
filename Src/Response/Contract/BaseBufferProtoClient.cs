using System.Buffers;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Configuration;
using Xcsb.Handlers;
using Xcsb.Handlers.Buffered;
using Xcsb.Handlers.Direct;
using Xcsb.Helpers;

namespace Xcsb.Response.Contract;

internal class BaseBufferProtoClient
{
    protected internal readonly BufferProtoOut BufferProtoOut;
    protected internal readonly BufferProtoIn BufferProtoIn;

    internal BaseBufferProtoClient(ProtoIn protoIn, ProtoOut protoOut)
    {
        BufferProtoOut = new BufferProtoOut(protoOut);
        BufferProtoIn = new BufferProtoIn(protoIn);
    }

    internal void FlushBase(bool shouldThorw)
    {
        try
        {
            BufferProtoIn.ProtoIn.FlushSocket();
            var outProtoSequence = BufferProtoOut.Sequence;
            BufferProtoOut.Flush();
            BufferProtoIn.FlushSocket(BufferProtoIn.ProtoIn.Sequence, outProtoSequence, shouldThorw);
        }
        finally
        {
            BufferProtoOut.Reset();
        }
    }
}