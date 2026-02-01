using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Replies.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct ListHostsResponse : IXReply
{
    public readonly ResponseHeader<AccessControlMode> ResponseHeader;
    public readonly uint Length;
    public readonly ushort NumberOfHosts;

    public bool Verify(in int sequence)
    {
        return (ResponseType)ResponseHeader.Reply == ResponseType.Reply && ResponseHeader.Sequence == sequence;
    }
    public AccessControlMode Mode => ResponseHeader.GetValue();
}