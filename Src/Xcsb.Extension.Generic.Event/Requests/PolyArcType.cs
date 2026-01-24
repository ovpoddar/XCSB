using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PolyArcType(uint drawable, uint gc, int arcLength)
{
    public readonly Opcode opcode = Opcode.PolyArc;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = (ushort)(3 + 3 * arcLength);
    public readonly uint Drawable = drawable;
    public readonly uint GC = gc;
}