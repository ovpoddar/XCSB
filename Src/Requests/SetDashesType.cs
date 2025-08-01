using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetDashesType(uint gc, ushort dashOffset, int dashLength)
{
    public readonly Opcode opcode = Opcode.SetDashes;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(3 + dashLength.AddPadding() / 4);
    public readonly uint GContext = gc;
    public readonly ushort DashOffset = dashOffset;
    public readonly ushort DashLength = (ushort)dashLength;
}