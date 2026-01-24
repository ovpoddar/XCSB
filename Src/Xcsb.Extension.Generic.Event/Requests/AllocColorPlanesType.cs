using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct AllocColorPlanesType(
    bool contiguous,
    uint colorMap,
    ushort colors,
    ushort reds,
    ushort greens,
    ushort blues)
{
    public readonly Opcode Opcode = Opcode.AllocColorPlanes;
    public readonly byte Contiguous = contiguous ? (byte)1 : (byte)0;
    public readonly ushort Length = 4;
    public readonly uint ColorMap = colorMap;
    public readonly ushort Colors = colors;
    public readonly ushort Reds = reds;
    public readonly ushort Greens = greens;
    public readonly ushort Blues = blues;
}