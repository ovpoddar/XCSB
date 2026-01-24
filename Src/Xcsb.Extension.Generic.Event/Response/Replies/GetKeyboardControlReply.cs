using Xcsb.Extension.Generic.Event.Response.Replies.Internals;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Replies;

public struct GetKeyboardControlReply
{
    public readonly ResponseType Reply;
    public readonly AutoRepeatMode AutoRepeatMode;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly uint LedMask;
    public readonly byte KeyClickPercent;
    public readonly byte BellPercent;
    public readonly ushort BellPitch;
    public readonly ushort BellDuration;
    public readonly byte[] Repeats = new byte[32];

    internal GetKeyboardControlReply(GetKeyboardControlResponse result)
    {
        unsafe
        {
            Reply = result.ResponseHeader.Reply;
            AutoRepeatMode = result.ResponseHeader.GetValue();
            Sequence = result.ResponseHeader.Sequence;
            Length = result.Length;
            LedMask = result.LedMask;
            KeyClickPercent = result.KeyClickPercent;
            BellPercent = result.BellPercent;
            BellPitch = result.BellPitch;
            BellDuration = result.BellDuration;
            new Span<byte>(result.Repeats, 32)
                .CopyTo(Repeats);
        }
    }
}