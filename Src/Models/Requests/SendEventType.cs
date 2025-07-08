using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models.Event;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 44)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SendEventType(bool propagate, uint destination, uint eventMask, XEvent evnt)
{
    public readonly Opcode OpCode = Opcode.SendEvent;
    public readonly byte Propagate = (byte)(propagate ? 1 : 0);
    public readonly ushort Length = 11;
    public readonly uint Destination = destination;
    public readonly uint EventMask = eventMask;
    public readonly XEvent XEvent = evnt;
}