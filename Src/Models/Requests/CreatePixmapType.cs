using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CreatePixmapType(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
{
    public readonly Opcode OpCode = Opcode.CreatePixmap;
    public readonly byte Depth = depth;
    public readonly ushort Length = 4;
    public readonly uint PixmapId = pixmapId;
    public readonly uint Drawable = drawable;
    public readonly ushort Width = width;
    public readonly ushort Height = height;
}