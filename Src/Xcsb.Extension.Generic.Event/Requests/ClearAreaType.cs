using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ClearAreaType(bool exposures, uint window, short x, short y, ushort width, ushort height)
{
    public readonly Opcode OpCode = Opcode.ClearArea;
    public readonly byte Exposures = (byte)(exposures ? 1 : 0);
    public readonly ushort Length = 4;
    public readonly uint Window = window;
    public readonly short X = x;
    public readonly short Y = y;
    public readonly ushort Width = width;
    public readonly ushort Height = height;
}