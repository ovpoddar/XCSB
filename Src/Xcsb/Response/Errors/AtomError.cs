using System.Runtime.InteropServices;
using Xcsb.Connection.Models.TypeInfo;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Response.Errors;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct AtomError : IXError
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint BadAtomId;
    public readonly ushort MinorOpcode;
    public readonly byte MajorOpcode;

    public readonly string GetErrorMessage() =>
        """
        A value for an ATOM argument does not name a defined
        ATOM.
        """;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.GetResponseType() == XResponseType.Error && ResponseHeader.Sequence == sequence
            && ResponseHeader.GetValue() == ErrorCode.Atom;
    }
}