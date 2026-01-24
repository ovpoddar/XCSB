using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Response.Replies.Internals;
using Xcsb.Helpers;
using Xcsb.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Replies;

public struct GetModifierMappingReply
{
    public readonly ResponseType Reply;
    public byte KeycodesPerModifier;
    public readonly ushort Sequence;
    public ulong[] Keycodes;

    internal GetModifierMappingReply(Span<byte> response)
    {
        ref readonly var context = ref response.AsStruct<GetModifierMappingResponse>();
        Reply = context.ResponseHeader.Reply;
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