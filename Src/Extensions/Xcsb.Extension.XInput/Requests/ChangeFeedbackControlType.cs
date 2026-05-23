using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeFeedbackControlType(byte majorOpCode, uint mask, byte deviceId, byte feedbackId)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.ChangeFeedbackControl;
    public readonly ushort Length = 3;
    public readonly uint Mask = mask;
    public readonly byte DeviceId = deviceId;
    public readonly byte FeedbackId = feedbackId;
}