using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GetPointerControlType()
{
    public readonly Opcode Opcode = Opcode.GetPointerControl;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 1;
}