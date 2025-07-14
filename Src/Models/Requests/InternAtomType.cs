using System.Runtime.InteropServices;
using Xcsb.Helpers;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
internal readonly struct InternAtomType(bool onlyIfExist, int atomNameLength)
{
    public readonly Opcode Opcode = Opcode.InternAtom;
    public readonly byte OnlyIfExists = (byte)(onlyIfExist ? 1 : 0);
    public readonly ushort Length = (ushort)(2 + atomNameLength.AddPadding() / 4);
    public readonly ushort NameLength = (ushort)atomNameLength;
    private readonly ushort _pad0;
}