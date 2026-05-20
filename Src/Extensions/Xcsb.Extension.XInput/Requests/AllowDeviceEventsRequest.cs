using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct AllowDeviceEventsRequest(byte majorOpCode)
{
    public readonly byte MajorOpCode = majorOpCode;
    public readonly OpCode Opcode = OpCode.AllowDeviceEvents;
    public readonly ushort Length;
    public readonly xcb_timestamp_t Time;
    public readonly byte Mode;
    public readonly byte DeviceId;
    public readonly byte Pad0;
}