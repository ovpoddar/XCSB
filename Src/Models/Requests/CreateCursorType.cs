using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CreateCursorType(
    uint cursorId,
    uint source,
    uint mask,
    ushort foreRed,
    ushort foreGreen,
    ushort foreBlue,
    ushort backRed,
    ushort backGreen,
    ushort backBlue,
    ushort x,
    ushort y)
{
    public readonly Opcode OpCode = Opcode.CreateCursor;
    private readonly byte _pad0;
    public readonly ushort Length = 8;
    public readonly uint CursorId = cursorId;
    public readonly uint Source = source;
    public readonly uint Mask = mask;
    public readonly ushort ForeRed = foreRed;
    public readonly ushort ForeGreen = foreGreen;
    public readonly ushort ForeBlue = foreBlue;
    public readonly ushort BackRed = backRed;
    public readonly ushort BackGreen = backGreen;
    public readonly ushort BackBlue = backBlue;
    public readonly ushort X = x;
    public readonly ushort Y = y;
}