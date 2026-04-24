using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CreateGlyphCursorType(
    uint cursorId,
    uint sourceFont,
    uint fontMask,
    char sourceChar,
    ushort charMask,
    ushort foreRed,
    ushort foreGreen,
    ushort foreBlue,
    ushort backRed,
    ushort backGreen,
    ushort backBlue)
{
    public readonly Opcode OpCode = Opcode.CreateGlyphCursor;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 8;
    public readonly uint CursorId = cursorId;
    public readonly uint SourceFont = sourceFont;
    public readonly uint FontMask = fontMask;
    public readonly char SourceChar = sourceChar;
    public readonly ushort CharMask = charMask;
    public readonly ushort ForeRed = foreRed;
    public readonly ushort ForeGreen = foreGreen;
    public readonly ushort ForeBlue = foreBlue;
    public readonly ushort BackRed = backRed;
    public readonly ushort BackGreen = backGreen;
    public readonly ushort BackBlue = backBlue;
}