using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct FreeColorsType(uint colormapId, uint planeMask, int pixelsLength)
{
    public readonly Opcode opcode = Opcode.FreeColors;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = (ushort)(3 + pixelsLength);
    public readonly uint ColorMapId = colormapId;
    public readonly uint PlaneMask = planeMask;
}