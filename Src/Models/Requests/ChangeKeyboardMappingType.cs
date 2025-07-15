using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeKeyboardMappingType(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode)
{
    public readonly Opcode OpCode = Opcode.ChangeKeyboardMapping;
    public readonly byte KeycodeCount = keycodeCount;
    public readonly ushort Length = (ushort)(2 + keycodeCount * keysymsPerKeycode);
    public readonly byte FirstKeycode = firstKeycode;
    public readonly byte KeysymsPerKeycode = keysymsPerKeycode;
}