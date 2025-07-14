using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CirculateWindowType(Direction direction, uint window)
{
    public readonly Opcode OpCode = Opcode.CirculateWindow;
    public readonly Direction Direction = direction;
    public readonly ushort Length = 2;
    public readonly uint Window = window;
}