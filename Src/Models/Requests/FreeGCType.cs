using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct FreeGCType(uint gc)
{
    public readonly Opcode OpCode = Opcode.FreeGC;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint GC = gc;
}
