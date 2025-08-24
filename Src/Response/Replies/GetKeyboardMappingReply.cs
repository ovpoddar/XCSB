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
        }
        // TODO: update the calling
        /*
        for (int i = 0; i < (count); ++i) {
            printf("Keycode %d: ", min + i);
            for (int j = 0; j < keysyms_per_keycode; ++j) {
                printf("0x%X ", keysyms[i * keysyms_per_keycode + j]);
            }
            printf("\n");
        }   
        */
    }
}