using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct GetDeviceDontPropagateListReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public uint[] Classes;

    public GetDeviceDontPropagateListReply(Span<byte> result)
    {
        ref readonly var response = ref result.AsStruct<GetDeviceDontPropagateListResponse>();
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        if (response.NumClasses == 0)
            Classes = Array.Empty<uint>();
        else
        {
            var responseLength = Unsafe.SizeOf<GetDeviceDontPropagateListResponse>();
            Classes = MemoryMarshal.Cast<byte, uint>(result[responseLength..]).ToArray();
        }
    }
}