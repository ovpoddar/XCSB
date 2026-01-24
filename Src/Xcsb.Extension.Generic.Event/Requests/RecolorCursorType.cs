using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct RecolorCursorType(
    uint cursorId,
    ushort foreRed,
    ushort foreGreen,
    ushort foreBlue,
    ushort backRed,
    ushort backGreen,
    ushort backBlue)
{
    public readonly Opcode OpCode = Opcode.RecolorCursor;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 5;
    public readonly uint CursorId = cursorId;
    public readonly ushort ForegroundRed = foreRed;
    public readonly ushort ForegroundGreen = foreGreen;
    public readonly ushort ForegroundBlue = foreBlue;
    public readonly ushort BackgroundRed = backRed;
    public readonly ushort BackgroundGreen = backGreen;
    public readonly ushort BackgroundBlue = backBlue;
}