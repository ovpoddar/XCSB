using Xcsb.Extension.Generic.Event.Response.Contract;
using Xcsb.Extension.Generic.Event.Response.Event;

namespace Xcsb.Response.Contract;

internal partial struct XResponse : IXBaseResponse
{
    public bool Verify(in int sequence)
    {
        return this.Sequence == sequence;
    }

    internal readonly XResponseType GetResponseType() => this.Reply switch
    {
        ResponseType.Reply => XResponseType.Reply,
        ResponseType.Error => XResponseType.Error,
        ResponseType.KeymapNotify => XResponseType.Notify,
        >= ResponseType.KeyPress and <= ResponseType.MappingNotify or (ResponseType)36 => XResponseType.Event,
        _ => XResponseType.Unknown
    };


    internal readonly unsafe ref readonly T As<T>() where T : struct
    {
        var responseType = GetResponseType();
        if ((responseType != XResponseType.Event && (typeof(IXEvent).IsAssignableFrom(typeof(T)) && typeof(T) != typeof(GenericEvent)))
            || (responseType != XResponseType.Error && typeof(IXError).IsAssignableFrom(typeof(T)))
            || (responseType != XResponseType.Reply && typeof(IXReply).IsAssignableFrom(typeof(T))))
            throw new InvalidCastException();

        if (responseType == XResponseType.Error)
            return ref _error.As<T>();

        if (responseType is XResponseType.Event or XResponseType.Notify or XResponseType.Unknown)
            return ref _event.As<T>();

        throw new InvalidCastException();
    }

    internal readonly unsafe Span<byte> bytes
    {
        get
        {
            fixed (byte* ptr = this._data)
                return new Span<byte>(ptr, 32);
        }
    }
}