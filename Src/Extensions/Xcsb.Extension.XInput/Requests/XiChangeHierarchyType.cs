using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct XiChangeHierarchyType(byte majorOpCode, byte numChanges, int changesLength)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.XiChangeHierarchy;
    public readonly ushort Length = (ushort)(2 + changesLength);
    public readonly byte NumChanges = numChanges;
}