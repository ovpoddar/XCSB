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
    internal GetKeyboardControlReply(GetKeyboardControlResponse result)
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
            this.Repeats = new Span<byte>(result.Repeats, 32).ToArray();
        }
    }
}