using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetDeviceFocusType(byte majorOpCode, uint focus, uint time, byte revertTo, byte deviceId)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.SetDeviceFocus;
    public readonly ushort Length = 4;
    public readonly uint Focus = focus;
    public readonly uint Time = time;
    public readonly byte RevertTo = revertTo;
    public readonly byte DeviceId = deviceId;
}