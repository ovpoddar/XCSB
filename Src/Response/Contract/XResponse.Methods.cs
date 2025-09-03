using System.Runtime.CompilerServices;
using Xcsb.Helpers;

namespace Xcsb.Response.Contract;

internal partial struct XResponse : IXBaseResponse
{
    public bool Verify(in int sequence)
    {
#if NETSTANDARD
        return (int)this.Reply <= 34 && (int)this.Reply >= 0;
#else
        return Enum.IsDefined(this.Reply);
#endif
    }

    internal readonly XResponseType GetResponseType() => this.Reply switch
    {
        ResponseType.Reply => XResponseType.Reply,
        ResponseType.Error => XResponseType.Error,
        ResponseType.KeymapNotify => XResponseType.Notify,
        >= ResponseType.KeyPress and <= ResponseType.MappingNotify or (ResponseType)36 => XResponseType.Event,
        _ => XResponseType.Invalid
    };


    internal readonly unsafe ref T As<T>() where T : struct
    {
        var responseType = GetResponseType();
        if ((responseType != XResponseType.Event && typeof(IXEvent).IsAssignableFrom(typeof(T)))
            || (responseType != XResponseType.Error && typeof(IXError).IsAssignableFrom(typeof(T))) 
            || (responseType != XResponseType.Reply && typeof(IXReply).IsAssignableFrom(typeof(T))))
            throw new InvalidCastException();

        if (responseType == XResponseType.Error)
            return ref _error.As<T>();

        if (responseType is XResponseType.Event or XResponseType.Notify)
            return ref _event.As<T>();

        throw new InvalidCastException();
    }
}