using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;
using Xcsb.Masks;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct UngrabDeviceKeyType(
    byte majorOpCode,
    uint grabWindow,
    ModifierMask modifiers,
    byte modifierDevice,
    byte key,
    byte grabbedDevice)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.UngrabDeviceKey;
    public readonly ushort Length = 4;
    public readonly uint GrabWindow = grabWindow;
    public readonly ModifierMask Modifiers = modifiers;
    public readonly byte ModifierDevice = modifierDevice;
    public readonly byte Key = key;
    public readonly byte GrabbedDevice = grabbedDevice;
}