using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct XiSetFocusType(byte majorOpCode, uint window, uint time, ushort deviceId)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.XiSetFocus;
    public readonly ushort Length = 4;
    public readonly uint Window = window;
    public readonly uint Time = time;
    public readonly ushort DeviceId = deviceId;
}