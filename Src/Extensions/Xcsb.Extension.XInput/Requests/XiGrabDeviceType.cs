using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct XiGrabDeviceType(
    byte majorOpCode,
    uint window,
    uint time,
    uint cursor,
    InputDevice deviceId,
    GrabMode mode,
    GrabMode pairedDeviceMode,
    GrabOwner ownerEvents,
    ushort maskLen)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.XiGrabDevice;
    public readonly ushort Length = 6;
    public readonly uint Window = window;
    public readonly uint Time = time;
    public readonly uint Cursor = cursor;
    public readonly InputDevice DeviceId = deviceId;
    public readonly GrabMode Mode = mode;
    public readonly GrabMode PairedDeviceMode = pairedDeviceMode;
    public readonly GrabOwner OwnerEvents = ownerEvents;
    public readonly byte Pad0 = 0;
    public readonly ushort MaskLen = maskLen;
}