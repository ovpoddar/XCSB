using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;
using Xcsb.Extension.Generic.Event.Response.Contract;
using Xcsb.Extension.Generic.Event.Response.Replies.Internals;
using Xcsb.Helpers;

namespace Xcsb.Extension.Generic.Event.Response.Replies;

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