using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Response.Contract;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Response.Replies;

public readonly struct ListPropertiesReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly ATOM[] Atoms;

    internal ListPropertiesReply(Span<byte> response)
    {
         ref var context = ref response.AsStruct<ListPropertiesResponse>();
        Reply = context.ResponseHeader.Reply;
        Sequence = context.ResponseHeader.Sequence;

        if (context.NumberOfProperties == 0)
            Atoms = [];
        else
        {
            var cursor = Unsafe.SizeOf<ListPropertiesResponse>();
            var length = context.NumberOfProperties * 4;
            Atoms = MemoryMarshal.Cast<byte, ATOM>(response.Slice(cursor, length)).ToArray();
        }
    }
}