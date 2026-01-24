using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ListFontsWithInfoType(int pattanLength, int maxNames)
{
    public readonly Opcode OpCode = Opcode.ListFontsWithInfo;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = (ushort)(2 + (pattanLength.AddPadding() / 4));
    public readonly ushort MaxNames = (ushort)maxNames;
    public readonly ushort PatternLength = (ushort)pattanLength;
}