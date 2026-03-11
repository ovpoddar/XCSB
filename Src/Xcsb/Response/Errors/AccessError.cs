using System.Runtime.InteropServices;
using Xcsb.Connection.Models.TypeInfo;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Response.Errors;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct AccessError : IXError
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint BadValue;
    public readonly ushort MinorOpcode;
    public readonly byte MajorOpcode;

    public readonly string GetErrorMessage() =>
        """
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
        """;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.GetResponseType() == XResponseType.Error && this.ResponseHeader.Sequence == sequence
            && ResponseHeader.GetValue() == ErrorCode.Access;
    }
}