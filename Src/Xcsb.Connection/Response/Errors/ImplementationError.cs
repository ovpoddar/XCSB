using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Connection.Response.Errors;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct ImplementationError : IXError
{
    public readonly ResponseHeader<ErrorCode> ResponseHeader;
    public readonly uint BadValue;
    public readonly ushort MinorOpcode;
    public readonly byte MajorOpcode;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.GetResponseType() == XResponseType.Error && this.ResponseHeader.Sequence == sequence;
    }
}
