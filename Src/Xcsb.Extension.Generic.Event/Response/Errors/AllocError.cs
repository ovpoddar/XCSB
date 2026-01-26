using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Errors;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct AllocError : IXError
{
    public readonly ResponseHeader<ErrorCode> ResponseHeader;
    public readonly uint BadValue;
    public readonly ushort MinorOpcode;
    public readonly byte MajorOpcode;
    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Error && this.ResponseHeader.Sequence == sequence;
    }
}
