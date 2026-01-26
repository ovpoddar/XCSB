using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;
using Xcsb.Extension.Generic.Event.Response.Contract;
using Xcsb.Extension.Generic.Event.Response.Replies.Internals;
using Xcsb.Helpers;

namespace Xcsb.Extension.Generic.Event.Response.Replies;

public readonly struct ListHostsReply
{
    public readonly ResponseType Reply;
    public readonly AccessControlMode Mode;
    public readonly ushort Sequence;
    public readonly ushort NumberOfHosts;
    public readonly uint[] Hosts;

    internal ListHostsReply(Span<byte> response)
    {
        ref readonly var context = ref response.AsStruct<ListHostsResponse>();
        Reply = context.ResponseHeader.Reply;
        Mode = context.ResponseHeader.GetValue();
        Sequence = context.ResponseHeader.Sequence;
        NumberOfHosts = context.NumberOfHosts;

        if (context.Length == 0)
            Hosts = [];
        else
        {
            var cursor = Unsafe.SizeOf<ListHostsResponse>();
            var length = (int)context.Length * 4;
            Hosts = MemoryMarshal.Cast<byte, uint>(response.Slice(cursor, length)).ToArray();
        }
    }
}