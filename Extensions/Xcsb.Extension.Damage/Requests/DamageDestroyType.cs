using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Damage.Models;

namespace Xcsb.Extension.Damage.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct DamageDestroyType(byte majorOpCode, uint damage)
{
    public readonly byte MajorOpCode = majorOpCode;
    public readonly OpCode OpCode = OpCode.Destroy;
    public readonly ushort Length = 2;
    public readonly uint Damage = damage;
}
