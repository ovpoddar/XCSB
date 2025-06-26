using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CopyPlaneType(uint srcDrawable, uint destDrawable, uint gc, ushort srcX, ushort srcY, ushort destX, ushort destY, ushort width, ushort height, uint bitPlane)
{
    public readonly Opcode OpCode = Opcode.CopyPlane;
    private readonly byte _pad0;
    public readonly ushort Length = 8;
    public readonly uint SourceDrawable = srcDrawable;
    public readonly uint DestinationDrawable = destDrawable;
    public readonly uint Gc = gc;
    public readonly ushort SourceX = srcX;
    public readonly ushort SourceY = srcY;
    public readonly ushort DestinationX = destX;
    public readonly ushort DestinationY = destY;
    public readonly ushort Width = width;
    public readonly ushort Height = height;
    public readonly uint BitPlane = bitPlane;
}