using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Response.Contract;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Response.Replies;

public struct GetModifierMappingReply
{
    public readonly ResponseType Reply;
    public byte KeycodesPerModifier;
    public readonly ushort Sequence;
    public ulong[] Keycodes;

    internal GetModifierMappingReply(Span<byte> response)
    {
        ref readonly var context = ref response.AsStruct<GetModifierMappingResponse>();
        Reply = (ResponseType)context.ResponseHeader.Reply;
        KeycodesPerModifier = context.ResponseHeader.GetValue();
        Sequence = context.ResponseHeader.Sequence;
        if (KeycodesPerModifier == 0)
            Keycodes = [];
        else
        {
            var cursor = Unsafe.SizeOf<GetModifierMappingResponse>();
            var length = KeycodesPerModifier * 8;
            Keycodes = MemoryMarshal.Cast<byte, ulong>(response[cursor..length]).ToArray();
        }
    }
}