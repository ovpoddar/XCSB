using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct DeviceBellType(
    byte majorOpCode,
    byte deviceId,
    byte feedbackId,
    byte feedbackClass,
    sbyte percent)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.DeviceBell;
    public readonly ushort Length = 2;
    public readonly byte DeviceId = deviceId;
    public readonly byte FeedbackId = feedbackId;
    public readonly byte FeedbackClass = feedbackClass;
    public readonly sbyte Percent = percent;
}