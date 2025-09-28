using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetFontPathType(ushort itemsLength, int requestLength)
{
    public readonly Opcode OpCode = Opcode.SetFontPath;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = (ushort)(2 + (requestLength / 4));
    public readonly ushort ItemsLength = itemsLength;
    private readonly ushort _pad1 = 0;
}