using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct XiDeletePropertyType(byte majorOpCode, ushort deviceId, ATOM property)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.XiDeleteProperty;
    public readonly ushort Length = 3;
    public readonly ushort DeviceId = deviceId;
    public readonly byte Pad0 = 0;
    public readonly ATOM Property = property;
}