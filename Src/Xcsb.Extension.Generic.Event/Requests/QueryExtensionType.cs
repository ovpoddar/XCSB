using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;
using Xcsb.Helpers;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct QueryExtensionType(ushort nameLength)
{
    public readonly Opcode Opcode = Opcode.QueryExtension;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = (ushort)(2 + nameLength.AddPadding() / 4);
    public readonly ushort NameLength = nameLength;
    private readonly ushort _pad1 = 0;
}