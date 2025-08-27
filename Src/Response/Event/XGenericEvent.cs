using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Helpers;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
public unsafe struct XGenericEvent : IXEvent
{
    [FieldOffset(0)] public readonly ResponseHeader<byte> ResponseHeader;
    [FieldOffset(4)] public fixed byte Data[28];


    [FieldOffset(0)] private fixed byte _data[32];

    public readonly bool Verify(in int sequence)
    {
        return this.ResponseHeader.Reply == ResponseType.GenericEvent && this.ResponseHeader.Sequence == sequence;
    }
}