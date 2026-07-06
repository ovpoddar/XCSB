using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;
using Xcsb.Masks;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct UngrabDeviceButtonType(
    byte majorOpCode,
    uint grabWindow,
    ModifierMask modifiers,
    byte modifierDevice,
    byte button,
    byte grabbedDevice)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.UngrabDeviceButton;
    public readonly ushort Length = 5;
    public readonly uint GrabWindow = grabWindow;
    public readonly ModifierMask Modifiers = modifiers;
    public readonly byte ModifierDevice = modifierDevice;
    public readonly byte Button = button;
    public readonly byte GrabbedDevice = grabbedDevice;
}