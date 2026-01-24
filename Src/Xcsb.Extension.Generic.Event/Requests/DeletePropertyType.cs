using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct DeletePropertyType(uint window, ATOM atom)
{
    public readonly Opcode opcode = Opcode.DeleteProperty;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 3;
    public readonly uint Window = window;
    public readonly ATOM Atom = atom;
}