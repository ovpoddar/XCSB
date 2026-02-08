using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct FillPolyBigType(
    uint drawable,
    uint gc,
    PolyShape shape,
    CoordinateMode coordinate,
    int pointsLength)
{
    public readonly Opcode opcode = Opcode.FillPoly;
    private readonly byte _pad0 = 0;
    private readonly ushort _pad = 0;
    public readonly uint Length = (uint)(5 + pointsLength);
    public readonly uint Drawable = drawable;
    public readonly uint Gc = gc;
    public readonly PolyShape Shape = shape;
    public readonly CoordinateMode Coordinate = coordinate;
    private readonly ushort _pad1 = 0;
}