using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Connection.Response.Errors;
using Xcsb.Response.Contract;
using Xcsb.Response.Event;

namespace Xcsb.Models;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
public unsafe struct XEvent
{
    [FieldOffset(0)] private XResponseNew _response;
    [FieldOffset(0)] private XEventType _eventType;
    [FieldOffset(1)] private ErrorCode _errorType;


    public readonly XEventType ReplyType =>
        (_response.GetResponseType(), _errorType) switch
        {
            (XResponseType.Error, ErrorCode.Request) => XEventType.RequestError,
            (XResponseType.Error, ErrorCode.Value) => XEventType.ValueError,
            (XResponseType.Error, ErrorCode.Window) => XEventType.WindowError,
            (XResponseType.Error, ErrorCode.Pixmap) => XEventType.PixmapError,
            (XResponseType.Error, ErrorCode.Atom) => XEventType.AtomError,
            (XResponseType.Error, ErrorCode.Cursor) => XEventType.CursorError,
            (XResponseType.Error, ErrorCode.Font) => XEventType.FontError,
            (XResponseType.Error, ErrorCode.Match) => XEventType.MatchError,
            (XResponseType.Error, ErrorCode.Drawable) => XEventType.DrawableError,
            (XResponseType.Error, ErrorCode.Access) => XEventType.AccessError,
            (XResponseType.Error, ErrorCode.Alloc) => XEventType.AllocError,
            (XResponseType.Error, ErrorCode.Colormap) => XEventType.ColormapError,
            (XResponseType.Error, ErrorCode.GContext) => XEventType.GContextError,
            (XResponseType.Error, ErrorCode.IDChoice) => XEventType.IDChoiceError,
            (XResponseType.Error, ErrorCode.Name) => XEventType.NameError,
            (XResponseType.Error, ErrorCode.Length) => XEventType.LengthError,
            (XResponseType.Error, ErrorCode.Implementation) => XEventType.ImplementationError,
            (XResponseType.Error, var unknown) => throw new ArgumentOutOfRangeException(nameof(_errorType), unknown,
                null),
            (XResponseType.Event or XResponseType.Notify, _) => _eventType,
            _ => XEventType.Unknown,
        };

    public readonly unsafe ref readonly T As<T>() where T : struct =>
        ref _response.Bytes.AsStruct<T>();

    public readonly GenericError? Error =>
        _response.GetResponseType() != XResponseType.Error
            ? null
            : _response.Bytes.AsStruct<GenericError>();

    public readonly GenericEvent? Event =>
        _response.GetResponseType() is XResponseType.Event or XResponseType.Notify
            ? null
            : _response.Bytes.AsStruct<GenericEvent>();

    public Span<byte> GetRawResponse()
    {
        return this.ReplyType != XEventType.Unknown ? [] : _response.Bytes;
    }

}