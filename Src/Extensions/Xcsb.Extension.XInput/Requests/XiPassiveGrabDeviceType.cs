using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct XiPassiveGrabDeviceType(
    byte majorOpCode,
    uint time,
    uint grabWindow,
    uint cursor,
    uint detail,
    InputDevice deviceId,
    ushort numModifiers,
    ushort maskLen,
    GrabType grabType,
    GrabMode22 grabMode,
    GrabMode pairedDeviceMode,
    GrabOwner ownerEvents)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.XiPassiveGrabDevice;
    public readonly ushort Length = 8;
    public readonly uint Time = time;
    public readonly uint GrabWindow = grabWindow;
    public readonly uint Cursor = cursor;
    public readonly uint Detail = detail;
    public readonly InputDevice DeviceId = deviceId;
    public readonly ushort NumModifiers = numModifiers;
    public readonly ushort MaskLen = maskLen;
    public readonly GrabType GrabType = grabType;
    public readonly GrabMode22 GrabMode = grabMode;
    public readonly GrabMode PairedDeviceMode = pairedDeviceMode;
    public readonly GrabOwner OwnerEvents = ownerEvents;
}