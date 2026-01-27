using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetPointerMappingType(int mapLenght)
{
    public readonly Opcode Opcode = Opcode.SetPointerMapping;
    public readonly byte MapLength = (byte)mapLenght;
    public readonly ushort Length = (ushort)((mapLenght.AddPadding() / 4) + 1);
}