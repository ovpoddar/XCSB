using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetDeviceModeType(byte majorOpCode, byte deviceId, byte mode)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.SetDeviceMode;
    public readonly ushort Length = 2;
    public readonly byte DeviceId = deviceId;
    public readonly byte Mode = mode;
}