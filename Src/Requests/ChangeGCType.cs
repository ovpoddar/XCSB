using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Masks;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeGCType(uint gc, GCMask mask, int argsLength)
{
    public readonly Opcode opcode = Opcode.ChangeGC;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(3 + argsLength);
    public readonly uint Gc = gc;
    public readonly GCMask Mask = mask;
}