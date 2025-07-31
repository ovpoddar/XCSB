using System.Net.Sockets;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

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
            this.Reply = result.Reply;
            this.AutoRepeatMode = result.AutoRepeatMode;
            this.Sequence = result.Sequence;
            this.Length = result.Length;
            this.LedMask = result.LedMask;
            this.KeyClickPercent = result.KeyClickPercent;
            this.BellPercent = result.BellPercent;
            this.BellPitch = result.BellPitch;
            this.BellDuration = result.BellDuration;
            this.Repeats = new byte[32];

            Span<byte> buffer = stackalloc byte[20];
            socket.ReceiveExact(buffer);
            
            new Span<byte>(result.Repeats, 12).CopyTo(this.Repeats[0..12]);
            buffer.CopyTo(this.Repeats[12..32]);
        }
    }
}