using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests.BigExtensation;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct FreeColorsBigType(uint colormapId, uint planeMask, int pixelsLength)
{
    public readonly Opcode opcode = Opcode.FreeColors;
    private readonly byte _pad0 = 0;
    private readonly ushort _pad = 0;
    public readonly uint Length = (uint)(4 + pixelsLength);
    public readonly uint ColorMapId = colormapId;
    public readonly uint PlaneMask = planeMask;
}