using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Errors;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public readonly struct ColormapError : IXError
{
    public readonly ResponseHeader<ErrorCode> ResponseHeader;
    public readonly uint BadResourceId;
    public readonly ushort MinorOpcode;
    public readonly byte MajorOpcode;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Error && ResponseHeader.Sequence == sequence;
    }
}