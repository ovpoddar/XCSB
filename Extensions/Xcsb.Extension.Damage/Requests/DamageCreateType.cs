using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Damage.Models;

namespace Xcsb.Extension.Damage.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct DamageCreateType(byte majorOpCode, uint damage, uint drawable, ReportLevel lavel)
{
    public readonly byte MajorOpCode = majorOpCode;
    public readonly OpCode OpCode = OpCode.Create;
    public readonly ushort Length = 4;
    public readonly uint Damage = damage;
    public readonly uint Drawable = drawable;
    public readonly ReportLevel ReportLavel = lavel;
}
