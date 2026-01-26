using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct AllocColorCellsType(bool contiguous, uint colorMap, ushort colors, ushort planes)
{
    public readonly Opcode Opcode = Opcode.AllocColorCells;
    public readonly byte Contiguous = contiguous ? (byte)1 : (byte)0;
    public readonly ushort Length = 3;
    public readonly uint ColorMap = colorMap;
    public readonly ushort Colors = colors;
    public readonly ushort Planes = planes;
}