using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct LastEvent(int sequence) : IXEvent
{
    public bool Verify(in int sequence)
    {
        return true;
    }

    public readonly EventType Reply = EventType.LastEvent;
    private readonly byte _pad = 0;
    public readonly ushort Sequence = (ushort)sequence;
}
