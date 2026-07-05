using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct XiGetPropertyType(
    byte majorOpCode,
    ushort deviceId,
    byte delete,
    ATOM property,
    ATOM type,
    uint offset,
    uint len)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.XiGetProperty;
    public readonly ushort Length = 6;
    public readonly ushort DeviceId = deviceId;
    public readonly byte Delete = delete;
    public readonly byte Pad0 = 0;
    public readonly ATOM Property = property;
    public readonly ATOM Type = type;
    public readonly uint Offset = offset;
    public readonly uint Len = len;
}