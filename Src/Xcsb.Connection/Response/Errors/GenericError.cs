using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Connection.Response.Errors;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
public unsafe struct GenericError : IXError
{
    [FieldOffset(0)] public readonly ResponseHeader<ErrorCode> ResponseHeader;
    [FieldOffset(4)] public fixed byte Data[28];

    [FieldOffset(0)] private fixed byte _data[32];

    public readonly bool Verify(in int sequence)
    {
        if (ResponseHeader.GetResponseType() != XResponseType.Error)
            return false;

        var errorType = this.ResponseHeader.GetValue();

#if NETSTANDARD
        if (!Enum.IsDefined(typeof(ErrorCode), errorType))
#else
        if (!Enum.IsDefined(errorType))
#endif
            return false;

        fixed (byte* ptr = this._data)
        {
            return errorType switch
            {
                ErrorCode.Request => new Span<byte>(ptr, 32).AsStruct<RequestError>().Verify(in sequence),
                ErrorCode.Value => new Span<byte>(ptr, 32).AsStruct<ValueError>().Verify(in sequence),
                ErrorCode.Window => new Span<byte>(ptr, 32).AsStruct<WindowError>().Verify(in sequence),
                ErrorCode.Pixmap => new Span<byte>(ptr, 32).AsStruct<PixmapError>().Verify(in sequence),
                ErrorCode.Atom => new Span<byte>(ptr, 32).AsStruct<AtomError>().Verify(in sequence),
                ErrorCode.Cursor => new Span<byte>(ptr, 32).AsStruct<CursorError>().Verify(in sequence),
                ErrorCode.Font => new Span<byte>(ptr, 32).AsStruct<FontError>().Verify(in sequence),
                ErrorCode.Match => new Span<byte>(ptr, 32).AsStruct<MatchError>().Verify(in sequence),
                ErrorCode.Drawable => new Span<byte>(ptr, 32).AsStruct<DrawableError>().Verify(in sequence),
                ErrorCode.Access => new Span<byte>(ptr, 32).AsStruct<AccessError>().Verify(in sequence),
                ErrorCode.Alloc => new Span<byte>(ptr, 32).AsStruct<AllocError>().Verify(in sequence),
                ErrorCode.Colormap => new Span<byte>(ptr, 32).AsStruct<ColormapError>().Verify(in sequence),
                ErrorCode.GContext => new Span<byte>(ptr, 32).AsStruct<GContextError>().Verify(in sequence),
                ErrorCode.IDChoice => new Span<byte>(ptr, 32).AsStruct<IDChoiceError>().Verify(in sequence),
                ErrorCode.Name => new Span<byte>(ptr, 32).AsStruct<NameError>().Verify(in sequence),
                ErrorCode.Length => new Span<byte>(ptr, 32).AsStruct<LengthError>().Verify(in sequence),
                ErrorCode.Implementation => new Span<byte>(ptr, 32).AsStruct<ImplementationError>().Verify(in sequence),
                _ => false
            };
        }
    }

    public readonly ref readonly T As<T>() where T : struct
    {
        var isNotValid = this.ResponseHeader.GetValue() switch
        {
            ErrorCode.Request when typeof(T) == typeof(RequestError) => false,
            ErrorCode.Value when typeof(T) == typeof(ValueError) => false,
            ErrorCode.Window when typeof(T) == typeof(WindowError) => false,
            ErrorCode.Pixmap when typeof(T) == typeof(PixmapError) => false,
            ErrorCode.Atom when typeof(T) == typeof(AtomError) => false,
            ErrorCode.Cursor when typeof(T) == typeof(CursorError) => false,
            ErrorCode.Font when typeof(T) == typeof(FontError) => false,
            ErrorCode.Match when typeof(T) == typeof(MatchError) => false,
            ErrorCode.Drawable when typeof(T) == typeof(DrawableError) => false,
            ErrorCode.Access when typeof(T) == typeof(AccessError) => false,
            ErrorCode.Alloc when typeof(T) == typeof(AllocError) => false,
            ErrorCode.Colormap when typeof(T) == typeof(ColormapError) => false,
            ErrorCode.GContext when typeof(T) == typeof(GContextError) => false,
            ErrorCode.IDChoice when typeof(T) == typeof(IDChoiceError) => false,
            ErrorCode.Name when typeof(T) == typeof(NameError) => false,
            ErrorCode.Length when typeof(T) == typeof(LengthError) => false,
            ErrorCode.Implementation when typeof(T) == typeof(ImplementationError) => false,
            _ when typeof(T) == typeof(GenericError) => false,
            _ => true
        };

        if (isNotValid)
            throw new InvalidCastException();

        fixed (byte* ptr = this._data)
            return ref new Span<byte>(ptr, 32).AsStruct<T>();
    }

    public readonly string GetErrorMessage()
    {
        return ResponseHeader.GetValue() switch
        {
            ErrorCode.Request => """
                                 The major or minor opcode does not specify a valid
                                 request.
                                 """,
            ErrorCode.Value => """
                               Some numeric value falls outside the range of values
                               accepted by the request. Unless a specific range is
                               specified for an argument, the full range defined by
                               the argument's type is accepted. Any argument defined
                               as a set of alternatives typically can generate
                               this error (due to the encoding).
                               """,
            ErrorCode.Window => """
                                A value for a WINDOW argument does not name a defined
                                WINDOW.
                                """,
            ErrorCode.Pixmap => """
                                A value for a PIXMAP argument does not name a defined
                                PIXMAP.
                                """,
            ErrorCode.Atom => """
                              A value for an ATOM argument does not name a defined
                              ATOM.
                              """,
            ErrorCode.Cursor => """
                                A value for a CURSOR argument does not name a defined
                                CURSOR.
                                """,
            ErrorCode.Font => """
                              A value for a FONT argument does not name a defined
                              FONT. A value for a FONTABLE argument does
                              not name a defined FONT or a defined GCONTEXT.
                              """,
            ErrorCode.Match => """
                               An InputOnly window is used as a DRAWABLE. In
                               a graphics request, the GCONTEXT argument does
                               not have the same root and depth as the destination
                               DRAWABLE argument. Some argument (or pair of arguments)
                               has the correct type and range, but it fails
                               to match in some other way required by the request.
                               """,
            ErrorCode.Drawable => """
                                  A value for a DRAWABLE argument does not name a
                                  defined WINDOW or PIXMAP.
                                  """,
            ErrorCode.Access => """
                                An attempt is made to grab a key/button combination
                                already grabbed by another client. An attempt
                                is made to free a colormap entry not allocated by the
                                client or to free an entry in a colormap that was created
                                with all entries writable. An attempt is made to
                                store into a read-only or an unallocated colormap entry.
                                An attempt is made to modify the access control
                                list from other than the local host (or otherwise authorized
                                client). An attempt is made to select an event
                                type that only one client can select at a time when another
                                client has already selected it.
                                """,
            ErrorCode.Alloc => """
                               The server failed to allocate the requested resource.
                               Note that the explicit listing of Alloc errors in request
                                only covers allocation errors at a very coarse
                               level and is not intended to cover all cases of a server
                               running out of allocation space in the middle of service.
                               The semantics when a server runs out of allocation
                               space are left unspecified, but a server may generate
                               an Alloc error on any request for this reason,
                               and clients should be prepared to receive such errors
                               and handle or discard them.
                               """,
            ErrorCode.Colormap => """
                                  A value for a COLORMAP argument does not name a
                                  defined COLORMAP.
                                  """,
            ErrorCode.GContext => """
                                  A value for a GCONTEXT argument does not name a
                                  defined GCONTEXT.
                                  """,
            ErrorCode.IDChoice => """
                                  The value chosen for a resource identifier either is
                                  not included in the range assigned to the client or is
                                  already in use.
                                  """,
            ErrorCode.Name => "A font or color of the specified name does not exist.",
            ErrorCode.Length => """
                                The length of a request is shorter or longer than that
                                required to minimally contain the arguments. The
                                length of a request exceeds the maximum length accepted
                                by the server.
                                """,
            ErrorCode.Implementation => """
                                        The server does not implement some aspect of the request.
                                        A server that generates this error for a core request
                                        is deficient. As such, this error is not listed for
                                        any of the requests, but clients should be prepared to
                                        receive such errors and handle or discard them.
                                        """,
            _ => "Unknown Error type."
        };
    }
}