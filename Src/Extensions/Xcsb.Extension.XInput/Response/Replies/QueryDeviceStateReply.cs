using System;
using System.Runtime.CompilerServices;
using Xcsb.Connection.Helpers;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct QueryDeviceStateReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public byte[] Keys;

    public QueryDeviceStateReply(Span<byte> result)
    {
        ref readonly var response = ref result.AsStruct<QueryDeviceStateResponse>();
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        
        if (response.NumClasses == 0)
            Keys = Array.Empty<byte>();
        else
        {
            var responseLength = Unsafe.SizeOf<QueryDeviceStateResponse>();
            Keys = result[responseLength..].ToArray();
        }
    }
}