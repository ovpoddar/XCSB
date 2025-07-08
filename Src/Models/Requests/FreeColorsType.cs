using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct FreeColorsType(uint colormapId, uint planeMask, int pixelsLength)
{
    public readonly Opcode opcode = Opcode.FreeColors;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(3 + pixelsLength);
    public readonly uint ColorMapId = colormapId;
    public readonly uint PlaneMask = planeMask;
}