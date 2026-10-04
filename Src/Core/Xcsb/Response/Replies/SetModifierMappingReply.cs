using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Replies;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct SetModifierMappingReply : IXReply<SetModifierMappingReply,SetModifierMappingReply>, IVerify
{
    public readonly ResponseHeader<ResponseType, MappingStatus> ResponseHeader;
    public readonly uint Length;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Reply && ResponseHeader.Verify(in sequence) &&
               Length == 0;
    }

    public MappingStatus Status => ResponseHeader.GetValue();
    public SetModifierMappingReply FromBytes(byte[] response)
    {
        return response.AsSpan().ToStruct<SetModifierMappingReply>();
    }
}