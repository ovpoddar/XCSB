using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct AllowDeviceEventsType(byte majorOpCode, uint time, byte mode, byte deviceId)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.AllowDeviceEvents;
    public readonly ushort Length = 3;
    public readonly uint Time = time;
    public readonly byte Mode = mode;
    public readonly byte DeviceId = deviceId;
}