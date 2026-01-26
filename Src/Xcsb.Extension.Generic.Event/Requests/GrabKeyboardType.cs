using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GrabKeyboardType(
    bool ownerEvents,
    uint grabWindow,
    uint timeStamp,
    GrabMode pointerMode,
    GrabMode keyboardMode)
{
    public readonly Opcode Opcode = Opcode.GrabKeyboard;
    public readonly byte OwnerEvents = ownerEvents ? (byte)1 : (byte)0;
    public readonly ushort Length = 4;
    public readonly uint GrabWindow = grabWindow;
    public readonly uint TimeStamp = timeStamp;
    public readonly GrabMode PointerMode = pointerMode;
    public readonly GrabMode KeyboardMode = keyboardMode;
    private readonly ushort _pad0 = 0;
}