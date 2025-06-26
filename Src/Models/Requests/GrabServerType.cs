using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GrabServerType()
{
    public readonly Opcode Opcode = Opcode.GrabServer;
    private readonly byte _pad0;
    public readonly ushort Length = 1;
}
