using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct XiGrabDeviceType(
    byte majorOpCode,
    uint window,
    uint time,
    uint cursor,
    ushort deviceId,
    byte mode,
    byte pairedDeviceMode,
    byte ownerEvents,
    ushort maskLen)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.XiGrabDevice;
    public readonly ushort Length = 6;
    public readonly uint Window = window;
    public readonly uint Time = time;
    public readonly uint Cursor = cursor;
    public readonly ushort DeviceId = deviceId;
    public readonly byte Mode = mode;
    public readonly byte PairedDeviceMode = pairedDeviceMode;
    public readonly byte OwnerEvents = ownerEvents;
    public readonly byte Pad0 = 0;
    public readonly ushort MaskLen = maskLen;
}