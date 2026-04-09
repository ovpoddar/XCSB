using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct UngrabDeviceType(byte majorOpCode, uint time, byte deviceId)
{
    public readonly byte MajorOpCode = majorOpCode;
    public readonly OpCode OpCode = OpCode.UngrabDevice;
    public readonly ushort Length = 3;
    public readonly uint Time = time;
    public readonly byte DeviceId = deviceId;
}