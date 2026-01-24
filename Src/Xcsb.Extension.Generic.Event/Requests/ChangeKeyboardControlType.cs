using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Masks;
using Xcsb.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeKeyboardControlType(KeyboardControlMask mask, int argsLength)
{
    public readonly Opcode OpCode = Opcode.ChangeKeyboardControl;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(2 + argsLength);
    public readonly KeyboardControlMask Mask = mask;
}