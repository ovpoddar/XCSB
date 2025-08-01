using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct StoreNamedColorType(ColorFlag mode, uint colormapId, uint pixels, int nameLength)
{
    public readonly Opcode OpCode = Opcode.StoreNamedColor;
    public readonly ColorFlag Mode = mode;
    public readonly ushort Length = (ushort)(4 + nameLength.AddPadding() / 4);
    public readonly uint ColorMapId = colormapId;
    public readonly uint Pixels = pixels;
    public readonly ushort NameLength = (ushort)nameLength;
}