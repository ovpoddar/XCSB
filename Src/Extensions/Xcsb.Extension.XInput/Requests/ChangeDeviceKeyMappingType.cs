using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeDeviceKeyMappingType(
    byte majorOpCode,
    byte deviceId,
    byte firstKeycode,
    byte keysymsPerKeycode,
    byte keycodeCount)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.ChangeDeviceKeyMapping;
    public readonly ushort Length = 2;
    public readonly byte DeviceId = deviceId;
    public readonly byte FirstKeycode = firstKeycode;
    public readonly byte KeysymsPerKeycode = keysymsPerKeycode;
    public readonly byte KeycodeCount = keycodeCount;
}