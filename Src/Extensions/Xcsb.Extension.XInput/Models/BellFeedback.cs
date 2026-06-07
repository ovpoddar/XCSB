using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Extension.XInput.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public unsafe struct BellFeedback(byte feedbackId, byte percent, short pitch, short duration) : IFeedback
{
    public readonly FeedbackClass ClassId { get; } = FeedbackClass.Bell;
    public readonly byte FeedbackId { get; } = feedbackId;
    public readonly ushort Length { get; } = 3;
    public readonly byte Percent = percent;
    private fixed sbyte _pad0[3];
    public readonly short Pitch = pitch;
    public readonly short Duration = duration;
}