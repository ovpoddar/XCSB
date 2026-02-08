using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Masks;
using Xcsb.Models;

namespace Xcsb.Requests.BigExtensation;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeKeyboardControlBigType(KeyboardControlMask mask, int argsLength)
{
    public readonly Opcode OpCode = Opcode.ChangeKeyboardControl;
    private readonly byte _pad0;
    private readonly ushort _pad = 0;
    public readonly uint Length = (uint)(3 + argsLength);
    public readonly KeyboardControlMask Mask = mask;
}