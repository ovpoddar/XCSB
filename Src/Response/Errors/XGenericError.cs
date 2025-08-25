using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Event;
using Xcsb.Helpers;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Errors;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
internal unsafe struct XGenericError : IXError
{
    [FieldOffset(0)] public readonly ResponseHeader<ErrorCode> ResponseHeader;
    [FieldOffset(4)] public fixed byte Data[28];


    [FieldOffset(0)] private fixed byte _data[32];

    public readonly T? To<T>() where T : struct, IXError
    {
        fixed (byte* ptr = this._data)
        {
            var result = new Span<byte>(ptr, 32).ToStruct<T>();
            if (this.ResponseHeader.Reply == ResponseType.Error &&
#if NETSTANDARD
            Enum.IsDefined(typeof(ErrorCode), this.ResponseHeader.GetValue())
#else
            Enum.IsDefined(this.ResponseHeader.GetValue())
#endif
            )
                return result;
            return null;
        }
    }

    public readonly bool Verify(in int sequence)
    {
#if NETSTANDARD
        return this.ResponseHeader.Reply == ResponseType.Error && Enum.IsDefined(typeof(ErrorCode), this.ResponseHeader.GetValue()) && this.ResponseHeader.Sequence == sequence;
#else
        return this.ResponseHeader.Reply == ResponseType.Error && Enum.IsDefined<ErrorCode>(this.ResponseHeader.GetValue()) && this.ResponseHeader.Sequence == sequence;
#endif
    }
}
