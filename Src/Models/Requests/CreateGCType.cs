using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Masks;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CreateGCType(uint gc, uint drawable, GCMask mask, int argsLength)
{
    public readonly Opcode OpCode = Opcode.CreateGC;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(4 + argsLength);
    public readonly uint Gc = gc;
    public readonly uint Drawable = drawable;
    public readonly GCMask Mask = mask;
}