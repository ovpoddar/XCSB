using Xcsb.Helpers;
namespace Xcsb.Response.Contract;

internal partial struct XResponse<T> : IXBaseResponse
{
    public unsafe T1? GetEvent<T1>() where T1 : struct
    {
        if (!this.IsEvent())
            return null;

        fixed (byte* ptr = this._data)
            return new Span<byte>(ptr, 32).ToStruct<T1>();
    }

    public unsafe T1? GetError<T1>() where T1 : struct
    {
        if (!this.IsError())
            return null;

        fixed (byte* ptr = this._data)
            return new Span<byte>(ptr, 32).ToStruct<T1>();
    }

    public unsafe T1? GetReply<T1>() where T1 : struct
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