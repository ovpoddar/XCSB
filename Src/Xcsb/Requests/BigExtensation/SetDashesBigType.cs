using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Models;

namespace Xcsb.Requests.BigExtensation;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetDashesBigType(uint gc, ushort dashOffset, int dashLength)
{
    public readonly Opcode opcode = Opcode.SetDashes;
    private readonly byte _pad0 = 0;
    private readonly ushort _pad = 0;
    public readonly uint Length = (uint)(4 + dashLength.AddPadding() / 4);
    public readonly uint GContext = gc;
    public readonly ushort DashOffset = dashOffset;
    public readonly ushort DashLength = (ushort)dashLength;
}