using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;
using Xcsb.Connection.Helpers;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeDevicePropertyType(byte majorOpCode, ATOM property, ATOM type, byte deviceId, int format,
    PropertyMode mode, uint numItems)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.ChangeDeviceProperty;
    public readonly ushort Length = (ushort)(5 + (numItems * format).AddPadding() / 4);
    public readonly ATOM Property = property;
    public readonly ATOM Type = type;
    public readonly byte DeviceId = deviceId;
    public readonly byte Format = (byte)(format * 8);
    public readonly PropertyMode Mode = mode;
    public readonly byte Pad0 = 0;
    public readonly uint NumItems = numItems;
}