using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 28)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CopyAreaType(
    uint srcDrawable,
    uint destinationDrawable,
    uint gc,
    ushort srcX,
    ushort srcY,
    ushort destinationX,
    ushort destinationY,
    ushort width,
    ushort height)
{
    public readonly Opcode OpCode = Opcode.CopyArea;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 7;
    public readonly uint SourceDrawable = srcDrawable;
    public readonly uint DestinationDrawable = destinationDrawable;
    public readonly uint Gc = gc;
    public readonly ushort SourceX = srcX;
    public readonly ushort SourceY = srcY;
    public readonly ushort DestinationX = destinationX;
    public readonly ushort DestinationY = destinationY;
    public readonly ushort Width = width;
    public readonly ushort Height = height;
}