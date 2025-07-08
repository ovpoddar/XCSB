using System.Runtime.InteropServices;

namespace Xcsb.Models.Event;
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 31)]
public unsafe struct ErrorEvent
{
    public ErrorCode ErrorCode;
    public fixed byte Padding[6];
    public ushort MinorOpCode;
    public ushort MajorOpCode;
}

/*Access An attempt is made to grab a key/button combina-
        tion already grabbed by another client. An attempt
        is made to free a colormap entry not allocated by the
        client or to free an entry in a colormap that was cre-
        ated with all entries writable. An attempt is made to
        store into a read-only or an unallocated colormap en-
        try. An attempt is made to modify the access control
        list from other than the local host (or otherwise au-
        thorized client). An attempt is made to select an event
        type that only one client can select at a time when an-
        other client has already selected it.
Alloc   The server failed to allocate the requested resource.
        Note that the explicit listing of Alloc errors in re-
        quest only covers allocation errors at a very coarse
        level and is not intended to cover all cases of a server
        running out of allocation space in the middle of ser-
        vice. The semantics when a server runs out of alloca-
        tion space are left unspecified, but a server may gen-
        erate an Alloc error on any request for this reason,
        and clients should be prepared to receive such errors
        and handle or discard them.
Atom A value for an ATOM argument does not name a de-
        fined ATOM.
Colormap A value for a COLORMAP argument does not name a
        defined COLORMAP.
Cursor A value for a CURSOR argument does not name a de-
        fined CURSOR.
Drawable A value for a DRAWABLE argument does not name a
        defined WINDOW or PIXMAP.
Font A value for a FONT argument does not name a de-
        fined FONT. A value for a FONTABLE argument does
        not name a defined FONT or a defined GCONTEXT.
GContext A value for a GCONTEXT argument does not name a
        defined GCONTEXT.
IDChoice The value chosen for a resource identifier either is
        not included in the range assigned to the client or is
        already in use.
Implementation The server does not implement some aspect of the re-
        quest. A server that generates this error for a core re-
        quest is deficient. As such, this error is not listed for
        any of the requests, but clients should be prepared to
        receive such errors and handle or discard them.
Length The length of a request is shorter or longer than that
        required to minimally contain the arguments. The
        length of a request exceeds the maximum length ac-
        cepted by the server.
Match An InputOnly window is used as a DRAWABLE. In
        a graphics request, the GCONTEXT argument does
        not have the same root and depth as the destination
        DRAWABLE argument. Some argument (or pair of ar-
        guments) has the correct type and range, but it fails
        to match in some other way required by the request.
Name A font or color of the specified name does not exist.
Pixmap A value for a PIXMAP argument does not name a de-
        fined PIXMAP.
Request The major or minor opcode does not specify a valid
        request.
Value Some numeric value falls outside the range of values
        accepted by the request. Unless a specific range is
        specified for an argument, the full range defined by
        the argument's type is accepted. Any argument de-
        fined as a set of alternatives typically can generate
        this error (due to the encoding).
Window A value for a WINDOW argument does not name a de-
        fined WINDOW.
*/