using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CreateColormapType(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
{
    public readonly Opcode OpCode = Opcode.CreateColormap;
    public readonly ColormapAlloc Alloc = alloc;
    public readonly ushort Length = 4;
    public readonly uint ColorMapId = colormapId;
    public readonly uint Window = window;
    public readonly uint Visual = visual;
}