using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models.TypeInfo;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Errors;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public readonly struct ColormapError : IXError
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint BadResourceId;
    public readonly ushort MinorOpcode;
    public readonly byte MajorOpcode;

    public readonly string GetErrorMessage() =>
        """
        A value for a COLORMAP argument does not name a
        defined COLORMAP.
        """;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Error && ResponseHeader.Sequence == sequence
            && ResponseHeader.GetValue() == ErrorCode.Colormap;
    }
}