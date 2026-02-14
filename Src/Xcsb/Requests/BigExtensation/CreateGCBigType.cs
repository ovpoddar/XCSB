using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Masks;
using Xcsb.Models;

namespace Xcsb.Requests.BigExtensation;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CreateGCBigType(uint gc, uint drawable, GcMask mask, int argsLength)
{
    public readonly Opcode OpCode = Opcode.CreateGC;
    private readonly byte _pad0 = 0;
    private readonly ushort _pad = 0;
    public readonly uint Length = (uint)(5 + argsLength);
    public readonly uint Gc = gc;
    public readonly uint Drawable = drawable;
    public readonly GcMask Mask = mask;
}