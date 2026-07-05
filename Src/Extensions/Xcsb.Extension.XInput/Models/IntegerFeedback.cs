using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Extension.XInput.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct IntegerFeedback(byte feedbackId, uint intToDisplay) : IFeedback
{
    public readonly FeedbackClass ClassId { get; } = FeedbackClass.Integer;
    public readonly byte FeedbackId { get; } = feedbackId;
    public readonly ushort Length { get; } = 2;
    public readonly uint IntToDisplay = intToDisplay;
}