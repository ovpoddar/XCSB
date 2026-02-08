using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests.BigExtensation;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct StoreColorsBigType(uint colormapId, int itemLength)
{
    public readonly Opcode OpCode = Opcode.StoreColors;
    private readonly byte _pad0 = 0;
    private readonly ushort _pad = 0;
    public readonly uint Length = (uint)(3 + itemLength * 3);
    public readonly uint ColorMapId = colormapId;
}