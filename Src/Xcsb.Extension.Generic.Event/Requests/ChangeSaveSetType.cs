using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeSaveSetType(ChangeSaveSetMode changeSaveSetMode, uint window)
{
    public readonly Opcode OpCode = Opcode.ChangeSaveSet;
    public readonly ChangeSaveSetMode Mode = changeSaveSetMode;
    public readonly ushort Length = 2;
    public readonly uint Window = window;
}