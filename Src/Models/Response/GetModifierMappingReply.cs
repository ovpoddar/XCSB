using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

public struct GetModifierMappingReply
{
    public readonly byte Reply;
    public byte KeycodesPerModifier;
    public readonly ushort Sequence;
    public ulong[] Keycodes;

    internal GetModifierMappingReply(GetModifierMappingResponse result, Socket socket)
    {
        this.Reply = result.Reply;
        this.KeycodesPerModifier = result.KeycodesPerModifier;
        this.Sequence = result.Sequence;
        if (result.Length == 0)
            this.Keycodes = [];
        else
        {
            var requiredSize = (int)result.Length * 8;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer[0..requiredSize]);
            this.Keycodes = MemoryMarshal.Cast<byte, ulong>(buffer[0..requiredSize]).ToArray();
        }
    }
}