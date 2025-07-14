using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetClipRectanglesType(
    ClipOrdering ordering,
    uint gc,
    ushort clipX,
    ushort clipY,
    int rectanglesLength)
{
    public readonly Opcode opcode = Opcode.SetClipRectangles;
    public readonly ClipOrdering ordering = ordering;
    public readonly ushort Length = (ushort)(3 + 2 * rectanglesLength);
    public readonly uint Gc = gc;
    public readonly ushort ClipX = clipX;
    public readonly ushort ClipY = clipY;
}