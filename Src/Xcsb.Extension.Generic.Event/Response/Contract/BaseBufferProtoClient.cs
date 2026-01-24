using Xcsb.Extension.Generic.Event.Handlers.Buffered;
using Xcsb.Handlers.Direct;

namespace Xcsb.Extension.Generic.Event.Response.Contract;

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