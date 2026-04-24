using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Damage.Models;

namespace Xcsb.Extension.Damage.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct DamageSubtractType(byte majorOpCode, uint damage, uint repair, uint parts)
{
    public readonly byte MajorOpCode = majorOpCode;
    public readonly OpCode OpCode = OpCode.Subtract;
    public readonly ushort Length = 4;
    public readonly uint Damage = damage;
    public readonly uint Repair = repair;
    public readonly uint Parts = parts;
}
