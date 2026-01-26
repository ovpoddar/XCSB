using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct BellType(sbyte percent)
{
    public readonly Opcode Opcode = Opcode.Bell;
    public readonly sbyte Percent = percent;
    public readonly ushort Length = 1;
}