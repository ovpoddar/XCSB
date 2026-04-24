using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Connection.Request;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ListExtensionsType()
{
    public readonly byte Opcode = 99;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 1;
}