using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ImageText16Type(uint drawable, uint gc, short x, short y, int textLength)
{
    public readonly Opcode OpCode = Opcode.ImageText16;
    public readonly byte TextLength = (byte)textLength;
    public readonly ushort Length = (ushort)(4 + (2 * textLength).AddPadding() / 4);
    public readonly uint Drawable = drawable;
    public readonly uint GC = gc;
    public readonly short X = x;
    public readonly short Y = y;
}