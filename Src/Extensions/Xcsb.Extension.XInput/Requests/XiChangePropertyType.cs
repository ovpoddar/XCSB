using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct XiChangePropertyType(
    byte majorOpCode,
    ushort deviceId,
    byte mode,
    byte format,
    ATOM property,
    ATOM type,
    uint numItems)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.XiChangeProperty;
    public readonly ushort Length = 5;
    public readonly ushort DeviceId = deviceId;
    public readonly byte Mode = mode;
    public readonly byte Format = format;
    public readonly ATOM Property = property;
    public readonly ATOM Type = type;
    public readonly uint NumItems = numItems;
}