using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GetDevicePropertyType(
    byte majorOpCode,
    ATOM property,
    ATOM type,
    uint offset,
    uint len,
    byte deviceId,
    byte delete)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.GetDeviceProperty;
    public readonly ushort Length = 6;
    public readonly ATOM Property = property;
    public readonly ATOM Type = type;
    public readonly uint Offset = offset;
    public readonly uint Len = len;
    public readonly byte DeviceId = deviceId;
    public readonly byte Delete = delete;
}