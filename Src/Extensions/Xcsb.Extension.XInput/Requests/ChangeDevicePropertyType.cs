using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeDevicePropertyType(
    byte majorOpCode,
    ATOM property,
    ATOM type,
    byte deviceId,
    byte format,
    byte mode,
    uint numItems)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.ChangeDeviceProperty;
    public readonly ushort Length = 5;
    public readonly ATOM Property = property;
    public readonly ATOM Type = type;
    public readonly byte DeviceId = deviceId;
    public readonly byte Format = format;
    public readonly byte Mode = mode;
    public readonly byte Pad0 = 0;
    public readonly uint NumItems = numItems;
}