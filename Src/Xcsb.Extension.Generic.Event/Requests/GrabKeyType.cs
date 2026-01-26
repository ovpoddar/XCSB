using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Masks;
using Xcsb.Extension.Generic.Event.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal unsafe struct GrabKeyType(
    bool exposures,
    uint grabWindow,
    ModifierMask mask,
    byte keycode,
    GrabMode pointerMode,
    GrabMode keyboardMode)
{
    public readonly Opcode OpCode = Opcode.GrabKey;
    public readonly byte Exposures = (byte)(exposures ? 1 : 0);
    public readonly ushort Length = 4;
    public readonly uint GrabWindow = grabWindow;
    public readonly ModifierMask ModifierMask = mask;
    public readonly byte KeyCode = keycode;
    public readonly GrabMode PointerMode = pointerMode;
    public readonly GrabMode KeyboardMode = keyboardMode;
    private fixed byte _pad0[3];
}