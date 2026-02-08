using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Models;

namespace Xcsb.Requests.BigExtensation;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ImageText8BigType(uint drawable, uint gc, short x, short y, int textLength)
{
    public readonly Opcode OpCode = Opcode.ImageText8;
    public readonly byte TextLength = (byte)textLength;
    private readonly ushort _pad = 0;
    public readonly uint Length = (uint)(5 + textLength.AddPadding() / 4);
    public readonly uint Drawable = drawable;
    public readonly uint GC = gc;
    public readonly short X = x;
    public readonly short Y = y;
}