using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct XiSelectEventsType(byte majorOpCode, uint window, ushort numMask, int length)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.XiSelectEvents;
    public readonly ushort Length = (ushort)(3 + length);
    public readonly uint Window = window;
    public readonly ushort NumMask = numMask;
}