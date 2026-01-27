using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Response.Contract;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Response.Replies;

public readonly struct GetKeyboardMappingReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly uint[] Keysyms;
    public readonly byte KeyPerKeyCode;

    internal GetKeyboardMappingReply(Span<byte> response, byte count)
    {
        ref readonly var context = ref response.AsStruct<GetKeyboardMappingResponse>();
        if (context.ResponseHeader.GetValue() * count != context.Length)
            throw new InvalidOperationException("Unknown reply");

        Reply = context.ResponseHeader.Reply;
        Sequence = context.ResponseHeader.Sequence;
        KeyPerKeyCode = context.ResponseHeader.GetValue();

        if (KeyPerKeyCode == 0)
            Keysyms = [];
        else
        {
            var cursor = Unsafe.SizeOf<GetKeyboardMappingResponse>();
            var length = (int)context.Length * 4;
            response = response.Slice(cursor, length);
            Keysyms = MemoryMarshal.Cast<byte, uint>(response).ToArray();
            // todo: implement try a custom reader of span and span or similar structure dimantion should be extra to
            // store but it's not a big deal' because it does not map to any native types
            // Keysyms = new uint[count][];
            // for (var i = 0; i < count; i++)
            //     Keysyms[i] = MemoryMarshal.Cast<byte, uint>(buffer.Slice(i * (KeyPerKeyCode * 4), KeyPerKeyCode * 4)).ToArray();
        }
    }
}