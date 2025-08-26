using Xcsb.Helpers;

namespace Xcsb.Response.Contract;

internal partial struct XResponse<T> : IXBaseResponse, IGenericResponse
{
    public unsafe T1? ToEvent<T1>() where T1 : struct, IXEvent
    {
        if (GetResponseType() != XResponseType.Event)
            return null;

        fixed (byte* ptr = this._data)
            return new Span<byte>(ptr, 32).ToStruct<T1>();
    }

    public unsafe T1? ToError<T1>() where T1 : struct, IXError
    {
        if (GetResponseType() != XResponseType.Error)
            return null;

        fixed (byte* ptr = this._data)
            return new Span<byte>(ptr, 32).ToStruct<T1>();
    }

    public unsafe T1? ToReply<T1>() where T1 : struct, IXReply
    {
        if (GetResponseType() != XResponseType.Reply)
            return null;

        fixed (byte* ptr = this._data)
            return new Span<byte>(ptr, 32).ToStruct<T1>();
    }

    public bool Verify(in int sequence)
    {
#if NETSTANDARD
        return (int)this.ResponseHeader.Reply <= 36 && (int)this.ResponseHeader.Reply >= 0;
#else
        return Enum.IsDefined<ResponseType>(this.ResponseHeader.Reply);
#endif
    }

    public XResponseType GetResponseType() => this.ResponseHeader.Reply switch
    {
        ResponseType.Reply => XResponseType.Reply,
        ResponseType.Error => XResponseType.Error,
        ResponseType.KeymapNotify => XResponseType.Notify,
        >= ResponseType.KeyPress and <= ResponseType.LastEvent => XResponseType.Event,
        _ => XResponseType.Invalid
    };
}