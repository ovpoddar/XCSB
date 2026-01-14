using Xcsb.Response.Errors;

namespace Xcsb.Models.Infrastructure.Exceptions;

public sealed class XEventException : Exception
{
    internal XEventException(GenericError error, string methodName = "")
        : base(GetErrorMessage(error.ResponseHeader.GetValue())) =>
        base.Source = methodName;

    private static string GetErrorMessage(ErrorCode errorCode)
    {
        return errorCode switch
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