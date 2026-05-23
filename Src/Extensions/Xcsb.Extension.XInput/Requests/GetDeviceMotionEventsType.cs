using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GetDeviceMotionEventsType(byte majorOpCode, uint start, uint stop, byte deviceId)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.GetDeviceMotionEvents;
    public readonly ushort Length = 4;
    public readonly uint Start = start;
    public readonly uint Stop = stop;
    public readonly byte DeviceId = deviceId;
}