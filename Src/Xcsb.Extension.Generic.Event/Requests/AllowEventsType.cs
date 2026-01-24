using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct AllowEventsType(EventsMode mode, uint time)
{
    public readonly Opcode OpCode = Opcode.AllowEvents;
    public readonly EventsMode Mode = mode;
    public readonly ushort Length = 2;
    public readonly uint Time = time;
}