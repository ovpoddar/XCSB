using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GetKeyboardMappingType(byte firstKeycode, byte count)
{
    public readonly Opcode Opcode = Opcode.GetKeyboardMapping;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 2;
    public readonly byte FirstKeycode = firstKeycode;
    public readonly byte Count = count;
    private readonly byte _pad1 = 0;
}