using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PolyPointType(CoordinateMode coordinate, uint drawable, uint gc, int pointsLength)
{
    public readonly Opcode opcode = Opcode.PolyPoint;
    public readonly CoordinateMode Coordinate = coordinate;
    public readonly ushort Length = (ushort)(3 + pointsLength);
    public readonly uint Drawable = drawable;
    public readonly uint Gc = gc;
}