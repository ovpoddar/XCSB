using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GrabDeviceKeyType(byte majorOpCode, uint grabWindow, ushort modifiers, byte modifierDevice,
    byte grabbedDevice, byte key, byte thisDeviceMode, byte otherDeviceMode, byte ownerEvents, ushort numClasses)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.GrabDeviceKey;
    public readonly ushort Length = (ushort)(5 + numClasses);
    public readonly uint GrabWindow = grabWindow;
    public readonly ushort NumClasses = numClasses;
    public readonly ushort Modifiers = modifiers;
    public readonly byte ModifierDevice = modifierDevice;
    public readonly byte GrabbedDevice = grabbedDevice;
    public readonly byte Key = key;
    public readonly byte ThisDeviceMode = thisDeviceMode;
    public readonly byte OtherDeviceMode = otherDeviceMode;
    public readonly byte OwnerEvents = ownerEvents;
}