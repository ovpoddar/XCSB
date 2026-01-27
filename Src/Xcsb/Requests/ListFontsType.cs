using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ListFontsType(int patternLength, int maxNames)
{
    public readonly Opcode Opcode = Opcode.ListFonts;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = (ushort)(2 + patternLength.AddPadding() / 4);
    public readonly ushort MaxNames = (ushort)maxNames;
    public readonly ushort PatternLength = (ushort)patternLength;

}