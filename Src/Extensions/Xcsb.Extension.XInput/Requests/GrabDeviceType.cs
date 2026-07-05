using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GrabDeviceType(
    byte majorOpCode,
    uint grabWindow,
    uint time,
    ushort numClasses,
    byte thisDeviceMode,
    byte otherDeviceMode,
    byte ownerEvents,
    byte deviceId)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.GrabDevice;
    public readonly ushort Length = 5;
    public readonly uint GrabWindow = grabWindow;
    public readonly uint Time = time;
    public readonly ushort NumClasses = numClasses;
    public readonly byte ThisDeviceMode = thisDeviceMode;
    public readonly byte OtherDeviceMode = otherDeviceMode;
    public readonly byte OwnerEvents = ownerEvents;
    public readonly byte DeviceId = deviceId;
}