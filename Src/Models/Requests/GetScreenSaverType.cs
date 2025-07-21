using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GetScreenSaverType()
{
    public readonly Opcode Opcode = Opcode.GetScreenSaver;
    private readonly byte _pad0;
    public readonly ushort Length = 1;
}