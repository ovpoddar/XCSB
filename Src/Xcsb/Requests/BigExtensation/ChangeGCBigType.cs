using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Masks;
using Xcsb.Models;

namespace Xcsb.Requests.BigExtensation;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeGCBigType(uint gc, GCMask mask, int argsLength)
{
    public readonly Opcode opcode = Opcode.ChangeGC;
    private readonly byte _pad0 = 0;
    private readonly ushort _pad =0;
    public readonly uint Length = (uint)(4 + argsLength);
    public readonly uint Gc = gc;
    public readonly GCMask Mask = mask;
}