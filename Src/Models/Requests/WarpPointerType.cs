using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct WarpPointerType(
    uint srcWindow,
    uint destinationWindow,
    short srcX,
    short srcY,
    ushort srcWidth,
    ushort srcHeight,
    short destinationX,
    short destinationY)
{
    public readonly Opcode OpCode = Opcode.WarpPointer;
    private readonly byte _pad0;
    public readonly ushort Length = 6;
    public readonly uint SrcWindow = srcWindow;
    public readonly uint DestinationWindow = destinationWindow;
    public readonly short SrcX = srcX;
    public readonly short SrcY = srcY;
    public readonly ushort SrcWidth = srcWidth;
    public readonly ushort SrcHeight = srcHeight;
    public readonly short DestinationX = destinationX;
    public readonly short DestinationY = destinationY;
}