using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct XiChangeCursorType(byte majorOpCode, uint window, uint cursor, ushort deviceId)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.XiChangeCursor;
    public readonly ushort Length = 4;
    public readonly uint Window = window;
    public readonly uint Cursor = cursor;
    public readonly ushort DeviceId = deviceId;
}