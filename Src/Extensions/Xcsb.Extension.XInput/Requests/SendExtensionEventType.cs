using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SendExtensionEventType(byte majorOpCode, uint destination, byte deviceId, bool propagate,
    byte numEvents, ushort numClasses)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.SendExtensionEvent;
    public readonly ushort Length = (ushort)(4 + (numEvents * 8) + numClasses );
    public readonly uint Destination = destination;
    public readonly byte DeviceId = deviceId;
    public readonly byte Propagate = propagate ? (byte)1 : (byte)0;
    public readonly ushort NumClasses = numClasses;
    public readonly byte NumEvents = numEvents;
}