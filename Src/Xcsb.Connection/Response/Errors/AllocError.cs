using System.Runtime.InteropServices;
using Xcsb.Connection.Models.TypeInfo;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Connection.Response.Errors;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct AllocError : IXError
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint BadValue;
    public readonly ushort MinorOpcode;
    public readonly byte MajorOpcode;

    public readonly string GetErrorMessage() =>
        """
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
        """;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.GetResponseType() == XResponseType.Error && this.ResponseHeader.Sequence == sequence
            && ResponseHeader.GetValue() == ErrorCode.Alloc;
    }
}
