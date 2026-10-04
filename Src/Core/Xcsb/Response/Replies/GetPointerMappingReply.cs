using System.Runtime.CompilerServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Response.Replies;

public readonly struct GetPointerMappingReply : IXReply<GetPointerMappingReply, GetPointerMappingResponse>
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly byte[] Map;

    internal GetPointerMappingReply(Span<byte> response)
    {
        ref readonly var context = ref response.AsStruct<GetPointerMappingResponse>();
        Reply = (ResponseType)context.ResponseHeader.Reply;
        Sequence = context.ResponseHeader.Sequence;
        if (context.ResponseHeader.GetValue() == 0)
            Map = [];
        else
        {
            var cursor = Unsafe.SizeOf<GetPointerMappingResponse>();
            Map = response.Slice(cursor, context.ResponseHeader.GetValue()).ToArray();
        }
    }
    
    public GetPointerMappingReply FromBytes(byte[] response)
    {
        return new GetPointerMappingReply(response);
    }
}