using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetInputFocusType(InputFocusMode mode, uint focus, uint time)
{
    public readonly Opcode opcode = Opcode.SetInputFocus;
    public readonly InputFocusMode Mode = mode;
    public readonly ushort Length = 3;
    public readonly uint Focus = focus;
    public readonly uint Time = time;
}