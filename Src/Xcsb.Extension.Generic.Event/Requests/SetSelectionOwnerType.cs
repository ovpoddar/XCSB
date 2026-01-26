using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetSelectionOwnerType(uint owner, ATOM atom, uint timestamp)
{
    public readonly Opcode OpCode = Opcode.SetSelectionOwner;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 4;
    public readonly uint Owner = owner;
    public readonly ATOM ATOM = atom;
    public readonly uint TimeStamp = timestamp;
}