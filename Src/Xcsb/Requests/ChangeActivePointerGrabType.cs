using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeActivePointerGrabType(uint cursor, uint time, ushort mask)
{
    public readonly Opcode OpCode = Opcode.ChangeActivePointerGrab;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 4;
    public readonly uint Cursor = cursor;
    public readonly uint Time = time;
    public readonly ushort Mask = mask;
    private readonly ushort _pad1 = 0;
}