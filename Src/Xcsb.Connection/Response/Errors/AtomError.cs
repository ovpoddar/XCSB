using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Connection.Response.Errors;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public readonly struct AtomError : IXError
{
    public readonly ResponseHeader<ErrorCode> ResponseHeader;
    public readonly uint BadAtomId;
    public readonly ushort MinorOpcode;
    public readonly byte MajorOpcode;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.GetResponseType() == XResponseType.Error && ResponseHeader.Sequence == sequence;
    }
}