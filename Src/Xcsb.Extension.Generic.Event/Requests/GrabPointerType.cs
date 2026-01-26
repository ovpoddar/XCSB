using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GrabPointerType(
    bool ownerEvents,
    uint grabWindow,
    ushort mask,
    GrabMode pointerMode,
    GrabMode keyboardMode,
    uint confineTo,
    uint cursor,
    uint timeStamp)
{
    public readonly Opcode OpCode = Opcode.GrabPointer;
    public readonly byte OwnerEvents = (byte)(ownerEvents ? 1 : 0);
    public readonly ushort Length = 6;
    public readonly uint GrabWindow = grabWindow;
    public readonly ushort Mask = mask;
    public readonly GrabMode PointerMode = pointerMode;
    public readonly GrabMode KeyboardMode = keyboardMode;
    public readonly uint ConfineTo = confineTo;
    public readonly uint Cursor = cursor;
    public readonly uint TimeStamp = timeStamp;
}