using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetSelectionOwnerType(uint owner, uint atom, uint timestamp)
{
    public readonly Opcode OpCode = Opcode.SetSelectionOwner;
    private readonly byte _pad0;
    public readonly ushort Length = 4;
    public readonly uint Owner = owner;
    public readonly uint Atom = atom;
    public readonly uint Timestamp = timestamp;
}