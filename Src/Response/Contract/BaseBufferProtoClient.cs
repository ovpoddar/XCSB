using System.Buffers;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Handlers;
using Xcsb.Helpers;

namespace Xcsb.Response.Contract;

internal class BaseBufferProtoClient
{
    protected internal readonly BufferProtoOut BufferProtoOut;
    protected internal readonly BufferProtoIn BufferProtoIn;
    
    protected readonly XcbClientConfiguration _configuration;

    internal BaseBufferProtoClient(ProtoIn protoIn, ProtoOut protoOut, XcbClientConfiguration configuration)
    {
        BufferProtoOut = new BufferProtoOut(protoOut);
        BufferProtoIn = new BufferProtoIn(protoIn);
        _configuration = configuration;
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