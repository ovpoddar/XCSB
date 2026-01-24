using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PolyText8Type(uint drawable, uint gc, ushort x, ushort y, int textLength)
{
    public readonly Opcode OpCode = Opcode.PolyText8;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = (ushort)(4 + textLength.AddPadding() / 4);
    public readonly uint Drawable = drawable;
    public readonly uint Gc = gc;
    public readonly ushort X = x;
    public readonly ushort Y = y;
}