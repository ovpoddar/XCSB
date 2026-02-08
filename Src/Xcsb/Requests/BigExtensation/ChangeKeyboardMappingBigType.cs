using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests.BigExtensation;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeKeyboardMappingBigType(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode)
{
    public readonly Opcode OpCode = Opcode.ChangeKeyboardMapping;
    public readonly byte KeycodeCount = keycodeCount;
    private readonly ushort _pad = 0;
    public readonly uint Length = (uint)(3 + keycodeCount * keysymsPerKeycode);
    public readonly byte FirstKeycode = firstKeycode;
    public readonly byte KeysymsPerKeycode = keysymsPerKeycode;
    private readonly ushort _pad0 = 0;
}