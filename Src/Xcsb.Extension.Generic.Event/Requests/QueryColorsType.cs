using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct QueryColorsType(uint colorMap, int pixelsLength)
{
    public readonly Opcode Opcode = Opcode.QueryColors;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = (ushort)(2 + pixelsLength);
    public readonly uint ColorMap = colorMap;
}