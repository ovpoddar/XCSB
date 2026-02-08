using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests.BigExtensation;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PolyRectangleBigType(uint drawable, uint gc, int rectanglesLength)
{
    public readonly Opcode opcode = Opcode.PolyRectangle;
    private readonly byte _pad0 = 0;
    private readonly ushort _pad = 0;
    public readonly uint Length = (uint)(4 + rectanglesLength * 2);
    public readonly uint Drawable = drawable;
    public readonly uint Gc = gc;
}