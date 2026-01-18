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

    internal BaseBufferProtoClient(ProtoIn protoIn, ProtoOut protoOut, XcbClientConfiguration configuration)
    {
        BufferProtoOut = new BufferProtoOut(protoOut, configuration);
        BufferProtoIn = new BufferProtoIn(protoIn, configuration);
    }

    protected void FlushBase(bool shouldThorw)
    {
        try
        {
            BufferProtoIn.ProtoIn.FlushSocket();
            var outProtoSequence = BufferProtoOut.ProtoOut.Sequence;
            BufferProtoOut.Flush();
            BufferProtoIn.FlushSocket(BufferProtoIn.ProtoIn.Sequence, outProtoSequence, shouldThorw);
        }
        finally
        {
            BufferProtoOut.Reset();
        }
    }
}