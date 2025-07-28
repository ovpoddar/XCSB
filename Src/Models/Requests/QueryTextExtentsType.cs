using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct QueryTextExtentsType(uint font, int strLength)
{
    public readonly Opcode Opcode = Opcode.QueryTextExtents;
    public readonly byte IsOddLength = (byte)(strLength & 1);
    public readonly ushort Length = (ushort)(2 + (2 * strLength).AddPadding() / 4);
    public readonly uint Font = font;
}