using Xcsb.Helpers;
namespace Xcsb.Response.Contract;

internal partial struct XResponse<T> : IXBaseResponse, IGenericResponse
{
    public unsafe T1? ToEvent<T1>() where T1 : struct, IXEvent
    {
        if (!this.IsEvent())
            return null;

        fixed (byte* ptr = this._data)
            return new Span<byte>(ptr, 32).ToStruct<T1>();
    }

    public unsafe T1? ToError<T1>() where T1 : struct, IXError
    {
        if (!this.IsError())
            return null;

        fixed (byte* ptr = this._data)
            return new Span<byte>(ptr, 32).ToStruct<T1>();
    }

    public unsafe T1? ToReply<T1>() where T1 : struct, IXReply
    {
        if (!this.IsReply())
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

    public bool IsEvent() =>
        (int)this.ResponseHeader.Reply <= 2 && (int)this.ResponseHeader.Reply >= 36;

    public bool IsError() =>
        this.ResponseHeader.Reply == ResponseType.Error;

    public bool IsReply() =>
        this.ResponseHeader.Reply == ResponseType.Reply;

}