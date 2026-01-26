using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct UnMapSubwindowsType(uint window)
{
    public readonly Opcode OpCode = Opcode.UnmapSubwindows;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 2;
    public readonly uint Window = window;
}