using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;
using Xcsb.Masks;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GrabDeviceKeyType(byte majorOpCode, uint grabWindow, ModifierMask modifiers, byte modifierDevice,
    byte grabbedDevice, byte key, GrabMode thisDeviceMode, GrabMode otherDeviceMode, bool ownerEvents, ushort numClasses)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.GrabDeviceKey;
    public readonly ushort Length = (ushort)(5 + numClasses);
    public readonly uint GrabWindow = grabWindow;
    public readonly ushort NumClasses = numClasses;
    public readonly ModifierMask Modifiers = modifiers;
    public readonly byte ModifierDevice = modifierDevice;
    public readonly byte GrabbedDevice = grabbedDevice;
    public readonly byte Key = key;
    public readonly GrabMode ThisDeviceMode = thisDeviceMode;
    public readonly GrabMode OtherDeviceMode = otherDeviceMode;
    public readonly byte OwnerEvents = ownerEvents ? (byte)1 : (byte)0;
}