using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetModifierMappingType(int keycodesLength)
{
    public readonly Opcode Opcode = Opcode.SetModifierMapping;
    public readonly byte KeycodesPerModifier = (byte)keycodesLength;
    public readonly ushort Length = (ushort)(1 + 2 * keycodesLength);
}