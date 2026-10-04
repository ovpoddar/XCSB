using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Replies;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GetInputFocusReply : IXReply<GetInputFocusReply, GetInputFocusReply>, IVerify
{
    public readonly ResponseHeader<ResponseType, InputFocusMode> ResponseHeader;
    public readonly uint Length;
    public readonly uint Focus;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Reply && ResponseHeader.Verify(in sequence) &&
               Length == 0;
    }

    public InputFocusMode Mode => ResponseHeader.GetValue();
    public GetInputFocusReply FromBytes(byte[] response)
    {
        return response.AsSpan().ToStruct<GetInputFocusReply>();
    }
}