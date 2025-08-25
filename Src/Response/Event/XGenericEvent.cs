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

    public readonly T? To<T>() where T : struct, IXEvent
    {

        fixed (byte* ptr = this._data)
        {
            var result = new Span<byte>(ptr, 32).ToStruct<T>();
            if (
#if NETSTANDARD
            Enum.IsDefined(typeof(ResponseType), ResponseHeader.Reply)
#else
            Enum.IsDefined(ResponseHeader.Reply)
#endif
            )
                return result;
            return null;
        }
    }

    public readonly bool Verify(in int sequence)
    {
#if NETSTANDARD
        return Enum.IsDefined(typeof(ResponseType), ResponseHeader.Reply) && this.ResponseHeader.Sequence == sequence;
#else
        return Enum.IsDefined(ResponseHeader.Reply) && this.ResponseHeader.Sequence == sequence;
#endif
    }
}