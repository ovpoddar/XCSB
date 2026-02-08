using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests.BigExtensation;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetClipRectanglesBigType(
    ClipOrdering ordering,
    uint gc,
    ushort clipX,
    ushort clipY,
    int rectanglesLength)
{
    public readonly Opcode opcode = Opcode.SetClipRectangles;
    public readonly ClipOrdering ordering = ordering;
    private readonly ushort _pad = 0;
    public readonly uint Length = (uint)(4 + 2 * rectanglesLength);
    public readonly uint Gc = gc;
    public readonly ushort ClipX = clipX;
    public readonly ushort ClipY = clipY;
}