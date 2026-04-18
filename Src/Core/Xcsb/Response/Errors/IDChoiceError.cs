using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models.TypeInfo;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Errors;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct IDChoiceError : IXError
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint BadResourceId;
    public readonly ushort MinorOpcode;
    public readonly byte MajorOpcode;

    public readonly string GetErrorMessage() =>
        """
        The value chosen for a resource identifier either is
        not included in the range assigned to the client or is
        already in use.
        """;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Error && ResponseHeader.Sequence == sequence
            && ResponseHeader.GetValue() == ErrorCode.IdChoice;
    }
}
