using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public unsafe struct XGenericEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public fixed byte Data[28];
    public bool Verify(in int sequence)
    {
#if NETSTANDARD
        return Enum.IsDefined(typeof(ResponseType), ResponseHeader.Reply) && this.ResponseHeader.Sequence == sequence;
#else
        return Enum.IsDefined<ResponseType>(ResponseHeader.Reply) && this.ResponseHeader.Sequence == sequence;
#endif
    }
}