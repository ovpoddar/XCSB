using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CloseDeviceType(byte majorOpCode, byte deviceId)
{
    public readonly byte MajorOpCode = majorOpCode;
    public readonly OpCode OpCode = OpCode.CloseDevice;
    public readonly ushort Length = 2;
    public readonly byte DeviceID = deviceId;
}