using System.Runtime.CompilerServices;
using Xcsb.Extension.Generic.Event.Response.Replies.Internals;
using Xcsb.Helpers;
using Xcsb.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Replies;

public readonly struct GetImageReply
{
    public readonly ResponseType Reply;
    public readonly byte Depth;
    public readonly ushort Sequence;
    public readonly uint VisualId;
    public readonly byte[] Data;

    internal GetImageReply(Span<byte> response)
    {
        ref readonly var context = ref response.AsStruct<GetImageResponse>();
        Reply = context.ResponseHeader.Reply;
        Depth = context.ResponseHeader.GetValue();
        Sequence = context.ResponseHeader.Sequence;
        VisualId = context.VisualId;
        if (context.Length == 0)
            Data = [];
        else
        {
            var cursor = Unsafe.SizeOf<GetImageResponse>();
            Data = response.Slice(cursor, (int)(context.Length * 4)).ToArray();
        }
    }
}