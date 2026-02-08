using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests.BigExtensation;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PolyArcBigType(uint drawable, uint gc, int arcLength)
{
    public readonly Opcode opcode = Opcode.PolyArc;
    private readonly byte _pad0 = 0;
    private readonly ushort _pad = 0;
    public readonly uint Length = (uint)(4 + 3 * arcLength);
    public readonly uint Drawable = drawable;
    public readonly uint GC = gc;
}