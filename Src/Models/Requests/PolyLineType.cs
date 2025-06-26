using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PolyLineType(CoordinateMode coordinate, uint drawable, uint gc, int pointsLength)
{
    public readonly Opcode opcode = Opcode.PolyLine;
    public readonly CoordinateMode Coordinate = coordinate;
    public readonly ushort Length = (ushort)(3 + pointsLength);
    public readonly uint Drawable = drawable;
    public readonly uint Gc = gc;
}