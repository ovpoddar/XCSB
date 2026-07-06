using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeDeviceControlType(byte majorOpCode, DeviceControl controlId, byte deviceId)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.ChangeDeviceControl;
    public readonly ushort Length = 2;
    public readonly DeviceControl ControlId = controlId;
    public readonly byte DeviceId = deviceId;
}