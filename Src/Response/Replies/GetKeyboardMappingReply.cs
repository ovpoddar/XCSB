using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Response.Contract;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public readonly struct GetKeyboardMappingReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly uint[] Keysyms;
    public readonly byte KeyPerKeyCode;

    internal GetKeyboardMappingReply(GetKeyboardMappingResponse result, byte count, Socket socket)
    {
        if (result.ResponseHeader.GetValue() * count != result.Length)
            throw new InvalidOperationException("Invalid reply");

        Reply = result.ResponseHeader.Reply;
        Sequence = result.ResponseHeader.Sequence;
        KeyPerKeyCode = result.ResponseHeader.GetValue();

        if (result.ResponseHeader.GetValue() == 0)
            Keysyms = [];
        else
        {
            var requiredSize = (int)result.Length * 4;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer[0..requiredSize]);
            Keysyms = MemoryMarshal.Cast<byte, uint>(buffer[0..requiredSize]).ToArray();
            // todo: implement try a custom reader of span and span or similar structure dimantion should be extra to
            // store but it's not a big deal' because it does not map to any native types
            // Keysyms = new uint[count][];
            // for (var i = 0; i < count; i++)
            //     Keysyms[i] = MemoryMarshal.Cast<byte, uint>(buffer.Slice(i * (KeyPerKeyCode * 4), KeyPerKeyCode * 4)).ToArray();
        }
    }
}