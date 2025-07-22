using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetPointerMappingType(Span<byte> maps)
{
    public readonly Opcode Opcode = Opcode.SetPointerMapping;
    public readonly byte Length = (byte)maps.Length;
    public readonly ushort MapLength = (ushort)((maps.Length.AddPadding() / 4) + 1);
}