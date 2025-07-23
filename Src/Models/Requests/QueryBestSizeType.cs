using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct QueryBestSizeType(QueryShapeOf shape, uint drawable, ushort width, ushort height)
{
    public readonly Opcode Opcode = Opcode.QueryBestSize;
    public readonly QueryShapeOf Shape = shape;
    public readonly ushort Length = 3;
    public readonly uint Drawable = drawable;
    public readonly ushort Width = width;
    public readonly ushort Height = height;
}