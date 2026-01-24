using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct UngrabPointerType(uint time)
{
    public readonly Opcode OpCode = Opcode.UngrabPointer;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 2;
    public readonly uint Time = time;
}