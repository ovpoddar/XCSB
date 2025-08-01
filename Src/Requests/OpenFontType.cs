using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct OpenFontType(uint fontId, int fontLength)
{
    public readonly Opcode opcode = Opcode.OpenFont;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(3 + fontLength.AddPadding() / 4);
    public readonly uint FontId = fontId;
    public readonly ushort FontLength = (ushort)fontLength;
    private readonly ushort _pad1;
}