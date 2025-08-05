using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct AllocColorType(uint colorMap, ushort red, ushort green, ushort blue)
{
    public readonly Opcode OpCode = Opcode.AllocColor;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 4;
    public readonly uint ColorMap = colorMap;
    public readonly ushort Red = red;
    public readonly ushort Green = green;
    public readonly ushort Blue = blue;
    private readonly ushort _pad1 = 0;
}