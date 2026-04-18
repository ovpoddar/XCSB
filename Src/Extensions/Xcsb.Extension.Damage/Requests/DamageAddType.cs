using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Damage.Models;

namespace Xcsb.Extension.Damage.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct DamageAddType(byte majorOpCode, uint drawable, uint region)
{
    public readonly byte MajorOpCode = majorOpCode;
    public readonly OpCode OpCode = OpCode.Add;
    public readonly ushort Length = 3;
    public readonly uint Drawable = drawable;
    public readonly uint Region = region;
}
