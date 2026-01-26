using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GetFontPathType()
{
    public readonly Opcode OpCode = Opcode.GetFontPath;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 1;
}