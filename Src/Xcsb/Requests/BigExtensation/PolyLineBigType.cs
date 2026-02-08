using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests.BigExtensation;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PolyLineBigType(CoordinateMode coordinate, uint drawable, uint gc, int pointsLength)
{
    public readonly Opcode opcode = Opcode.PolyLine;
    public readonly CoordinateMode Coordinate = coordinate;
    public readonly ushort _pad = 0;
    public readonly uint Length = (uint)(4 + pointsLength);
    public readonly uint Drawable = drawable;
    public readonly uint Gc = gc;
}