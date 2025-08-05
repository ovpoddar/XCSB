using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GetAtomNameType(uint atom)
{
    public readonly Opcode Opcode = Opcode.GetAtomName;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 2;
    public readonly uint Atom = atom;
}