using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct KillClientType(uint resource)
{
    public readonly Opcode Opcode = Opcode.KillClient;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint Resource = resource;
}