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
        if (result.ResponseHeader.GetValue() * count != result.ResponseHeader.Length)
            throw new InvalidOperationException("Invalid reply");

        Reply = result.ResponseHeader.Reply;
        Sequence = result.ResponseHeader.Sequence;
        if (result.ResponseHeader.GetValue() == 0)
            Keysyms = [];
        else
        {
            var requiredSize = (int)result.ResponseHeader.Length * 4;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer[0..requiredSize]);
            Keysyms = MemoryMarshal.Cast<byte, uint>(buffer[0..requiredSize]).ToArray();
        }
    }
}