using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct QueryKeymapType()
{
    public readonly Opcode Opcode = Opcode.QueryKeymap;
    private readonly byte _pad0;
    public readonly ushort Length = 1;
}