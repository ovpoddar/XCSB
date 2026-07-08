using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Extension.XInput.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct PointerFeedback(byte feedbackId, short num, short threshold, short denom) : IFeedback
{
    public readonly FeedbackClass ClassId { get; } = FeedbackClass.Pointer;
    public readonly byte FeedbackId { get; } = feedbackId;
    public readonly ushort Length { get; } = 3;
    private readonly ushort _pad0 = 0;
    public readonly short Num = num;
    public readonly short Denom = denom;
    public readonly short Threshold = threshold;
}