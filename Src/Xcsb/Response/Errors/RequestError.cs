using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models.TypeInfo;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Errors;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct RequestError : IXError
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint BadValue;
    public readonly ushort MinorOpcode;
    public readonly byte MajorOpcode;

    public readonly string GetErrorMessage() =>
        "The major or minor opcode does not specify a valid request.";

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Error && this.ResponseHeader.Sequence == sequence
            && ResponseHeader.GetValue() == ErrorCode.Request;
    }
}