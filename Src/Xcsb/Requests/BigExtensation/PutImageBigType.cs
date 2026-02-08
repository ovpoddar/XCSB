using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Models;

namespace Xcsb.Requests.BigExtensation;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 28)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PutImageBigType(
    ImageFormatBitmap format,
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
    public readonly ImageFormatBitmap Format = format;
    private readonly ushort _pad = 0;
    public readonly uint Length = (uint)(7 + (dataLength.AddPadding() / 4));
    public readonly uint Drawable = drawable;
    public readonly uint Gc = gc;
    public readonly ushort Width = width;
    public readonly ushort Height = height;
    public readonly short X = x;
    public readonly short Y = y;
    public readonly byte LeftPad = leftPad;
    public readonly byte Depth = depth;
    private readonly byte _pad0 = 0;
}