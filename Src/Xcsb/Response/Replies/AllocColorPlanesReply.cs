using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Response.Contract;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Response.Replies;

public struct AllocColorPlanesReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly uint RedMask;
    public readonly uint GreenMask;
    public readonly uint BlueMask;
    public uint[] Pixels;

    internal AllocColorPlanesReply(Span<byte> response)
    {
        ref readonly var context = ref response.AsStruct<AllocColorPlanesResponse>();
        Reply = context.ResponseHeader.Reply;
        Sequence = context.ResponseHeader.Sequence;
        RedMask = context.RedMask;
        GreenMask = context.GreenMask;
        BlueMask = context.BlueMask;
        if (context.NumberOfPixels == 0)
            Pixels = [];
        else
        {
            var cursor = Unsafe.SizeOf<AllocColorPlanesResponse>();
            var length = (context.NumberOfPixels * 4);
            Debug.Assert(cursor + length == response.Length);
            Pixels = MemoryMarshal.Cast<byte, uint>(response.Slice(cursor, length)).ToArray();
        }
    }
}