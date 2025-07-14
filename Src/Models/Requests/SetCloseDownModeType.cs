using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetCloseDownModeType(CloseDownMode mode)
{
    public readonly Opcode Opcode = Opcode.SetCloseDownMode;
    public readonly CloseDownMode Mode = mode;
    public readonly ushort Length = 1;
}