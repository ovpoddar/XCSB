using System.Runtime.CompilerServices;
using Xcsb.Connection.Helpers;
using Xcsb.Response.Contract;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Response.Replies;

public readonly struct GetPropertyReply
{
    public readonly ResponseType Reply;
    public readonly byte Format;
    public readonly ushort Sequence;
    public readonly uint Type;
    public readonly byte[] Data;

    internal GetPropertyReply(Span<byte> response)
    {
        ref readonly var context = ref response.AsStruct<GetPropertyResponse>();
        Reply = (ResponseType)context.ResponseHeader.Reply;
        Format = context.ResponseHeader.GetValue();
        Sequence = context.ResponseHeader.Sequence;
        Type = context.Type;

        if (context.Length == 0)
            Data = [];
        else
        {
            var cursor = Unsafe.SizeOf<GetPropertyResponse>();
            var length = (int)context.Length * 4;
            Data = response.Slice(cursor, length).ToArray();
        }

    }
}