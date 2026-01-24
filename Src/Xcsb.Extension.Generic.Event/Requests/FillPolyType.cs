using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct FillPolyType(
    uint drawable,
    uint gc,
    PolyShape shape,
    CoordinateMode coordinate,
    int pointsLength)
{
    public readonly Opcode opcode = Opcode.FillPoly;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = (ushort)(4 + pointsLength);
    public readonly uint Drawable = drawable;
    public readonly uint Gc = gc;
    public readonly PolyShape Shape = shape;
    public readonly CoordinateMode Coordinate = coordinate;
    private readonly ushort _pad1 = 0;
}