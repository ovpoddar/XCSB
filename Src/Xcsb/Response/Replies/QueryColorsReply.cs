using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;
using Xcsb.Response.Contract;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Response.Replies;

public readonly struct QueryColorsReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly Pixel[] Colors;
    internal QueryColorsReply(Span<byte> response)
    {
        ref readonly var context = ref response.AsStruct<QueryColorsResponse>();
        Reply = context.ResponseHeader.Reply;
        Sequence = context.ResponseHeader.Sequence;
        if (context.NumberOfColors == 0)
            Colors = [];
        else
        {

            var cursor = Unsafe.SizeOf<QueryColorsResponse>();
            var length = context.NumberOfColors * Marshal.SizeOf<Pixel>();
            Colors = MemoryMarshal.Cast<byte, Pixel>(response.Slice(cursor, length)).ToArray();
        }
    }
}