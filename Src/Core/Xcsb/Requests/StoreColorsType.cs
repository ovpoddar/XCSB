using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct StoreColorsType(uint colormapId, int itemLength)
{
    public readonly Opcode OpCode = Opcode.StoreColors;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = (ushort)(2 + itemLength * 3);
    public readonly uint ColorMapId = colormapId;
}