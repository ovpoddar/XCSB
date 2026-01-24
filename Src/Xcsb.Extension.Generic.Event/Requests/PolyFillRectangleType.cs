using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PolyFillRectangleType(uint drawable, uint gc, int rectanglesLength)
{
    public readonly Opcode opcode = Opcode.PolyFillRectangle;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = (ushort)(3 + 2 * rectanglesLength);
    public readonly uint Drawable = drawable;
    public readonly uint GC = gc;
}