using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public readonly struct GetKeyboardMappingReply
{
    public readonly byte Reply;
    public readonly ushort Sequence;
    public readonly uint[] Keysyms;

    internal GetKeyboardMappingReply(GetKeyboardMappingResponse result, byte count, Socket socket)
    {
        if (result.KeyPerKeyCode * count != result.Length)
            throw new InvalidOperationException("Invalid reply");

        Reply = result.Reply;
        Sequence = result.Sequence;
        if (result.KeyPerKeyCode == 0)
            Keysyms = [];
        else
        {
            var requiredSize = (int)result.Length * 4;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer[0..requiredSize]);
            Keysyms = MemoryMarshal.Cast<byte, uint>(buffer[0..requiredSize]).ToArray();
        }
    }
}