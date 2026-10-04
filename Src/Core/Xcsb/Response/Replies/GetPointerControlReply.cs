using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Replies;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GetPointerControlReply : IXReply<GetPointerControlReply, GetPointerControlReply>, IVerify
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint Length;
    public readonly ushort AccelNumerator;
    public readonly ushort AccelDenominator;
    public readonly ushort Threshold;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Reply && ResponseHeader.Verify(in sequence) &&
               Length == 0;
    }

    public GetPointerControlReply FromBytes(byte[] response)
    {
        return response.AsSpan().ToStruct<GetPointerControlReply>();
    }
}