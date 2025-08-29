using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Event;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

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
