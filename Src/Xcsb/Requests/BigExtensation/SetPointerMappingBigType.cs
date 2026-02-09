using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Models;

namespace Xcsb.Requests.BigExtensation;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetPointerMappingBigType(int mapLenght)
{
    public readonly Opcode Opcode = Opcode.SetPointerMapping;
    public readonly byte MapLength = (byte)mapLenght;
    private readonly ushort _pad = 0;
    public readonly uint Length = (uint)((mapLenght.AddPadding() / 4) + 2);
}