using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CopyColormapAndFreeType(uint colormapId, uint srcColormapId)
{
    public readonly Opcode opcode = Opcode.CopyColormapAndFree;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 3;
    public readonly uint ColorMapId = colormapId;
    public readonly uint SourceColorMapId = srcColormapId;
}