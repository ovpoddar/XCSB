using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct XiPassiveGrabDeviceType(
    byte majorOpCode,
    uint time,
    uint grabWindow,
    uint cursor,
    uint detail,
    ushort deviceId,
    ushort numModifiers,
    ushort maskLen,
    byte grabType,
    byte grabMode,
    byte pairedDeviceMode,
    byte ownerEvents)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.XiPassiveGrabDevice;
    public readonly ushort Length = 8;
    public readonly uint Time = time;
    public readonly uint GrabWindow = grabWindow;
    public readonly uint Cursor = cursor;
    public readonly uint Detail = detail;
    public readonly ushort DeviceId = deviceId;
    public readonly ushort NumModifiers = numModifiers;
    public readonly ushort MaskLen = maskLen;
    public readonly byte GrabType = grabType;
    public readonly byte GrabMode = grabMode;
    public readonly byte PairedDeviceMode = pairedDeviceMode;
    public readonly byte OwnerEvents = ownerEvents;
}