using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct XGenericEvent : IXBaseResponse
{
    public readonly EventType EventType;
    private readonly byte _pad0;
    public readonly ushort Sequence;

    public bool Verify(in int sequence)
    {
#if NETSTANDARD
        return Enum.IsDefined(typeof(EventType), EventType) && Sequence == sequence;
#else
        return Enum.IsDefined<EventType>(EventType) && Sequence == sequence;
#endif
    }
}