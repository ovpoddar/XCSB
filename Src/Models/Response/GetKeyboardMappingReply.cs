using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

public struct GetKeyboardMappingReply
{
    public readonly byte Reply;
    public readonly ushort Sequence;
    public readonly uint[] Keysyms;

    internal GetKeyboardMappingReply(GetKeyboardMappingResponse result, byte count, Socket socket)
    {
        if (result.KeyPerKeyCode * count != result.Length)
            throw new InvalidOperationException("Invalid reply");
        
        this.Reply = result.Reply;
        this.Sequence = result.Sequence;
        if (result.KeyPerKeyCode == 0)
            this.Keysyms = [];
        else
        {
            var requiredSize = (int)result.Length * 4;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer[0..requiredSize]);
            this.Keysyms = MemoryMarshal.Cast<byte, uint>(buffer[0..requiredSize]).ToArray();
        }
    }
}