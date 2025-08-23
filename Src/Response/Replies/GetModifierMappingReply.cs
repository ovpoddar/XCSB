using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Response.Contract;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public struct GetModifierMappingReply
{
    public readonly ResponseType Reply;
    public byte KeycodesPerModifier;
    public readonly ushort Sequence;
    public ulong[] Keycodes;

    internal GetModifierMappingReply(GetModifierMappingResponse result, Socket socket)
    {
        Reply = result.ResponseHeader.Reply;
        KeycodesPerModifier = result.ResponseHeader.GetValue();
        Sequence = result.ResponseHeader.Sequence;
        if (result.ResponseHeader.GetValue() == 0)
            Keycodes = [];
        else
        {
            var requiredSize = result.ResponseHeader.GetValue() * 8;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer[0..requiredSize]);
            Keycodes = MemoryMarshal.Cast<byte, ulong>(buffer[0..requiredSize]).ToArray();
        }
    }
}