using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GetMotionEventsType(uint window, uint startTime, uint endTime)
{
    public readonly Opcode OpCode = Opcode.GetMotionEvents;
    private readonly byte _pad0;
    public readonly ushort Length = 4;
    public readonly uint Window = window;
    public readonly uint StartTime = startTime;
    public readonly uint EndTime = endTime;
}