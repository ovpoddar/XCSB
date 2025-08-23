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
        // todo could be optimized with simple if check and int cast
        return Enum.IsDefined(typeof(ResponseType), this.Type) && Sequence == sequence;
#else
        return Enum.IsDefined<ResponseType>(Type) && Sequence == sequence;
#endif
    }

    public bool IsEvent() =>
        (int)this.Type <= 2 && (int)this.Type >= 36;

    public bool IsError() =>
        this.Type == ResponseType.Error;

    public bool IsReply() =>
        this.Type == ResponseType.Reply;

}