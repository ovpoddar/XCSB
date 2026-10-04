using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Replies;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GetSelectionOwnerReply : IXReply<GetSelectionOwnerReply,GetSelectionOwnerReply>, IVerify
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint Length;
    public readonly uint Owner;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Reply && ResponseHeader.Verify(in sequence) &&
               Length == 0;
    }

    public GetSelectionOwnerReply FromBytes(byte[] response)
    {
        return response.AsSpan().ToStruct<GetSelectionOwnerReply>();
    }
}