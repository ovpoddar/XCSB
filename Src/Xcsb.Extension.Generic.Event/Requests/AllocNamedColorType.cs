using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;
using Xcsb.Helpers;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct AllocNamedColorType(uint colorMap, int nameLength)
{
    public readonly Opcode Opcode = Opcode.AllocNamedColor;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = (ushort)(3 + nameLength.AddPadding() / 4);
    public readonly uint ColorMap = colorMap;
    public readonly ushort NameLength = (ushort)nameLength;
    private readonly ushort _pad1 = 0;
}