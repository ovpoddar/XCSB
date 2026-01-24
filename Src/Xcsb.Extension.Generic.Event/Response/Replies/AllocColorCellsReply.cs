using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Response.Replies.Internals;
using Xcsb.Helpers;
using Xcsb.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Replies;

public struct AllocColorCellsReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public uint[] Pixels;
    public ushort[] Masks;

    internal AllocColorCellsReply(Span<byte> response)
    {
        ref readonly var context = ref response.AsStruct<AllocColorCellsResponse>();
        Reply = context.ResponseHeader.Reply;
        Sequence = context.ResponseHeader.Sequence;

        var cursor = Unsafe.SizeOf<AllocColorCellsReply>();
        var length = (context.NumberOfPixels * 4);
        Pixels = context.NumberOfPixels == 0
            ? []
            : MemoryMarshal.Cast<byte, uint>(response.Slice(cursor, length)).ToArray();
        cursor += length;

        Masks = context.NumberOfMasks == 0
            ? []
            : MemoryMarshal.Cast<byte, ushort>(response[cursor..]).ToArray();
    }
}