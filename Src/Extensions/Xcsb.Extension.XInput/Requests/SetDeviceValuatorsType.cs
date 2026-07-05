using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetDeviceValuatorsType(byte majorOpCode, byte deviceId, byte firstValuator, byte numValuators)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.SetDeviceValuators;
    public readonly ushort Length = 2;
    public readonly byte DeviceId = deviceId;
    public readonly byte FirstValuator = firstValuator;
    public readonly byte NumValuators = numValuators;
}