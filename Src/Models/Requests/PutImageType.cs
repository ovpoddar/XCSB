using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PutImageType(
    ImageFormat format,
    uint drawable,
    uint gc,
    ushort width,
    ushort height,
    short x,
    short y,
    byte leftPad,
    byte depth,
    int dataLength)
{
    public readonly Opcode OpCode = Opcode.PutImage;
    public readonly ImageFormat Format = format;
    public readonly ushort Length = (ushort)(6 + dataLength.AddPadding() / 4);
    public readonly uint Drawable = drawable;
    public readonly uint Gc = gc;
    public readonly ushort Width = width;
    public readonly ushort Height = height;
    public readonly short X = x;
    public readonly short Y = y;
    public readonly byte LeftPad = leftPad;
    public readonly byte Depth = depth;
}