using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Masks;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct UngrabButtonType(Button button, uint grabWindow, ModifierMask modifier)
{
    public readonly Opcode opcode = Opcode.UngrabButton;
    public readonly Button Button = button;
    public readonly ushort Length = 3;
    public readonly uint GrabWindow = grabWindow;
    public readonly ModifierMask Modifier = modifier;
}