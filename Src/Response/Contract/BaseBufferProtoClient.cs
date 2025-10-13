using System.Buffers;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Handlers;
using Xcsb.Helpers;

namespace Xcsb.Response.Contract;

internal class BaseBufferProtoClient
{
    protected readonly BufferProtoOut BufferProtoOut;
    protected readonly BufferProtoIn BufferProtoIn; 
    
    internal BaseBufferProtoClient(ProtoIn protoIn, ProtoOut protoOut)
    {
        BufferProtoOut = new BufferProtoOut(protoOut);
        BufferProtoIn = new BufferProtoIn(protoIn);
    }

    protected void FlushBase(bool shouldThorw)
    {
        try
        {
            BufferProtoOut.Flush();
            BufferProtoIn.FlushSocket(BufferProtoIn.ProtoIn.Sequence, shouldThorw);
        }
        finally
        {
            BufferProtoOut.Reset();
        }
    }
}