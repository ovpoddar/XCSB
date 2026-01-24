using System.Runtime.CompilerServices;
using Xcsb.Extension.Generic.Event.Response.Replies.Internals;
using Xcsb.Helpers;
using Xcsb.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Replies;

public readonly struct GetPointerMappingReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly byte[] Map;

    internal GetPointerMappingReply(Span<byte> response)
    {
        ref readonly var context = ref response.AsStruct<GetPointerMappingResponse>();
        Reply = context.ResponseHeader.Reply;
        Sequence = context.ResponseHeader.Sequence;
        if (context.ResponseHeader.GetValue() == 0)
            Map = [];
        else
        {
            var cursor = Unsafe.SizeOf<GetPointerMappingResponse>();
            Map = response.Slice(cursor, context.ResponseHeader.GetValue()).ToArray();
        }
    }
}