using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PolyFillRectangleType(uint drawable, uint gc, int rectanglesLength)
{
    public readonly Opcode opcode = Opcode.PolyFillRectangle;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(3 + 2 * rectanglesLength);
    public readonly uint Drawable = drawable;
    public readonly uint GC = gc;
}