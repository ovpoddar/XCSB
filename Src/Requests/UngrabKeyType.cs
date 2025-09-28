using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Masks;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct UngrabKeyType(byte key, uint grabWindow, ModifierMask modifier)
{
    public readonly Opcode opcode = Opcode.UngrabKey;
    public readonly byte Key = key;
    public readonly ushort Length = 3;
    public readonly uint GrabWindow = grabWindow;
    public readonly ModifierMask Modifier = modifier;
    private readonly ushort _pad0 = 0;
}