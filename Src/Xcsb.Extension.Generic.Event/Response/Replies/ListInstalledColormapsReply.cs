using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Response.Contract;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Response.Replies;

public struct ListInstalledColormapsReply
{
    public ResponseType Reply;
    public ushort Sequence;
    public uint[] Colormap;

    internal ListInstalledColormapsReply(Span<byte> response)
    {
        ref readonly var context = ref response.AsStruct<ListInstalledColormapsResponse>();

        Reply = context.ResponseHeader.Reply;
        Sequence = context.ResponseHeader.Sequence;
        if (context.Length == 0)
            Colormap = [];
        else
        {
            var cursor = Unsafe.SizeOf<ListInstalledColormapsResponse>();
            var length = context.NumberOfColormaps * 4;
            Colormap = MemoryMarshal.Cast<byte, uint>(response.Slice(cursor, length)).ToArray();
        }
    }
}