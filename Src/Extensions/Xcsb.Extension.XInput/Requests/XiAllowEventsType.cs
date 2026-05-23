using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct XiAllowEventsType(
    byte majorOpCode,
    uint time,
    ushort deviceId,
    byte eventMode,
    uint touchId,
    uint grabWindow)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.XiAllowEvents;
    public readonly ushort Length = 5;
    public readonly uint Time = time;
    public readonly ushort DeviceId = deviceId;
    public readonly byte EventMode = eventMode;
    public readonly byte Pad0 = 0;
    public readonly uint TouchId = touchId;
    public readonly uint GrabWindow = grabWindow;
}