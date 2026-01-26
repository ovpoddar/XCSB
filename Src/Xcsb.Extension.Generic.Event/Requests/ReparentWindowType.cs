using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ReparentWindowType(uint window, uint parent, short x, short y)
{
    public readonly Opcode OpCode = Opcode.ReparentWindow;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 4;
    public readonly uint Window = window;
    public readonly uint Parent = parent;
    public readonly short X = x;
    public readonly short Y = y;
}