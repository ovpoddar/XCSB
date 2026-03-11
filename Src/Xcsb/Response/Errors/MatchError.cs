using System.Runtime.InteropServices;
using Xcsb.Connection.Models.TypeInfo;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Response.Errors;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct MatchError : IXError
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint BadValue;
    public readonly ushort MinorOpcode;
    public readonly byte MajorOpcode;

    public readonly string GetErrorMessage() =>
        """
        An InputOnly window is used as a DRAWABLE. In
        a graphics request, the GCONTEXT argument does
        not have the same root and depth as the destination
        DRAWABLE argument. Some argument (or pair of arguments)
        has the correct type and range, but it fails
        to match in some other way required by the request.
        """;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.GetResponseType() == XResponseType.Error && this.ResponseHeader.Sequence == sequence
            && ResponseHeader.GetValue() == ErrorCode.Match;
    }
}
