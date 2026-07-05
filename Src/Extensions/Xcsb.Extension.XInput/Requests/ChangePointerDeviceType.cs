using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangePointerDeviceType(byte majorOpCode, byte xAxis, byte yAxis, byte deviceId)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.ChangePointerDevice;
    public readonly ushort Length = 2;
    public readonly byte XAxis = xAxis;
    public readonly byte YAxis = yAxis;
    public readonly byte DeviceId = deviceId;
}