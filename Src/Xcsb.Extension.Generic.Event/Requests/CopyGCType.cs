using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Masks;
using Xcsb.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CopyGCType(uint srcGc, uint dstGc, GCMask mask)
{
    public readonly Opcode OpCode = Opcode.CopyGC;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 4;
    public readonly uint SourceGC = srcGc;
    public readonly uint DestinationGC = dstGc;
    public readonly GCMask GCMake = mask;
}