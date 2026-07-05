using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Extension.XInput.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct LedFeedback(byte feedbackId, uint ledMask, uint ledValues) : IFeedback
{
    public readonly FeedbackClass ClassId { get; } = FeedbackClass.Led;
    public readonly byte FeedbackId { get; } = feedbackId;
    public readonly ushort Length { get; } = 3;
    public readonly uint LedMask = ledMask;
    public readonly uint LedValues = ledValues;
}