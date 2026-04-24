using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Damage.Models;

namespace Xcsb.Extension.Damage.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct DamageQueryVersionType(byte majorOpCode, uint majorVersion, uint minorVersion)
{
    public readonly byte MajorOpCode = majorOpCode;
    public readonly OpCode OpCode = OpCode.QueryVersion;
    public readonly ushort Length = 3;
    public readonly uint MajorVersion = majorVersion;
    public readonly uint MinorVersion = minorVersion;
}
