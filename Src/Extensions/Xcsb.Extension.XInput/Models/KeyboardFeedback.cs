using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;

namespace Xcsb.Extension.XInput.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct KeyboardFeedback(byte feedbackId, byte key, byte autoRepeatMode, sbyte keyClickPercent,
    sbyte bellPercent, short bellPitch, short bellDuration, uint ledMask, uint ledValues) : IFeedback
{
    public readonly FeedbackClass ClassId { get; } = FeedbackClass.Keyboard;
    public readonly byte FeedbackId { get; } = feedbackId;
    public readonly ushort Length { get; } = 5;
    public readonly byte Key = key;
    public readonly byte AutoRepeatMode = autoRepeatMode;
    public readonly sbyte KeyClickPercent = keyClickPercent;
    public readonly sbyte BellPercent = bellPercent;
    public readonly short BellPitch = bellPitch;
    public readonly short BellDuration = bellDuration;
    public readonly uint LedMask = ledMask;
    public readonly uint LedValues = ledValues;
}