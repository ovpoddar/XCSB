using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CirculateWindowType(Circulate circulate, uint window)
{
    public readonly Opcode OpCode = Opcode.CirculateWindow;
    public readonly Circulate Circulate = circulate;
    public readonly ushort Length = 2;
    public readonly uint Window = window;
}