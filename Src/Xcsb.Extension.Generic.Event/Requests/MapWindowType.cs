using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct MapWindowType(uint window)
{
    public readonly Opcode OpCode = Opcode.MapWindow;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 2;
    public readonly uint Window = window;
}