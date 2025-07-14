using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PolyRectangleType(uint drawable, uint gc, int rectanglesLength)
{
    public readonly Opcode opcode = Opcode.PolyRectangle;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(3 + rectanglesLength * 2);
    public readonly uint Drawable = drawable;
    public readonly uint Gc = gc;
}