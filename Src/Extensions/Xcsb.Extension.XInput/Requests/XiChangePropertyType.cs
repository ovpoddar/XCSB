using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;
using Xcsb.Models;
using Xcsb.Connection.Helpers;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct XiChangePropertyType(byte majorOpCode, InputDevice deviceId, PropertyMode mode, byte format,
    ATOM property, ATOM type, uint numItems)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.XiChangeProperty;
    public readonly ushort Length = (ushort)(5 + (numItems * format).AddPadding() / 4);
    public readonly InputDevice DeviceId = deviceId;
    public readonly PropertyMode Mode = mode;
    public readonly byte Format = (byte)(format * 8);
    public readonly ATOM Property = property;
    public readonly ATOM Type = type;
    public readonly uint NumItems = numItems;
}