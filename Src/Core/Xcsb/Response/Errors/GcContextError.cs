using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models.TypeInfo;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Errors;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GcContextError : IXError<GcContextError>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint BadResourceId;
    public readonly ushort MinorOpcode;
    public readonly byte MajorOpcode;

    public GcContextError FromBytes(byte[] response)
    {
        return response.AsSpan().ToStruct<GcContextError>();
    }

    public readonly string GetErrorMessage() =>
        """
        A value for a GcCONTEXT argument does not name a
        defined GcCONTEXT.
        """;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Error && ResponseHeader.Sequence == sequence
            && ResponseHeader.GetValue() == ErrorCode.GcContext;
    }
}
