using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models.TypeInfo;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Errors;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct ValueError : IXError
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint BadValue;
    public readonly ushort MinorOpcode;
    public readonly byte MajorOpcode;

    public readonly string GetErrorMessage() =>
        """
        Some numeric value falls outside the range of values
        accepted by the request. Unless a specific range is
        specified for an argument, the full range defined by
        the argument's type is accepted. Any argument defined
        as a set of alternatives typically can generate
        this error (due to the encoding).
        """;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Error && ResponseHeader.Sequence == sequence
            && ResponseHeader.GetValue() == ErrorCode.Value;
    }
}
