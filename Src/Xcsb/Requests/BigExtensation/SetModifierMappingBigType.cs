using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests.BigExtensation;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetModifierMappingBigType(int keycodesLength)
{
    public readonly Opcode Opcode = Opcode.SetModifierMapping;
    public readonly byte KeycodesPerModifier = (byte)keycodesLength;
    private readonly ushort _pad = 0;
    public readonly uint Length = (uint)(2 + 2 * keycodesLength);
}