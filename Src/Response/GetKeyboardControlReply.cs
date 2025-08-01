using System.Net.Sockets;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public struct GetKeyboardControlReply
{
    public readonly byte Reply;
    public readonly AutoRepeatMode AutoRepeatMode;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly uint LedMask;
    public readonly byte KeyClickPercent;
    public readonly byte BellPercent;
    public readonly ushort BellPitch;
    public readonly ushort BellDuration;
    public byte[] Repeats;
    internal GetKeyboardControlReply(GetKeyboardControlResponse result, Socket socket)
    {
        unsafe
        {
            Reply = result.Reply;
            AutoRepeatMode = result.AutoRepeatMode;
            Sequence = result.Sequence;
            Length = result.Length;
            LedMask = result.LedMask;
            KeyClickPercent = result.KeyClickPercent;
            BellPercent = result.BellPercent;
            BellPitch = result.BellPitch;
            BellDuration = result.BellDuration;
            Repeats = new byte[32];

            Span<byte> buffer = stackalloc byte[20];
            socket.ReceiveExact(buffer);

            new Span<byte>(result.Repeats, 12).CopyTo(Repeats[0..12]);
            buffer.CopyTo(Repeats[12..32]);
        }
    }
}