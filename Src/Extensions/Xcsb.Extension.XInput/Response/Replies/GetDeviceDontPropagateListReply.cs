using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public readonly struct GetDeviceDontPropagateListReply : IXReply<GetDeviceDontPropagateListReply, GetDeviceDontPropagateListResponse>
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly uint[] Classes;

    internal GetDeviceDontPropagateListReply(Span<byte> result)
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

    public bool Verify(in int sequence)
    {
        throw new NotImplementedException();
    }

    public GetDeviceDontPropagateListReply FromBytes(Span<byte> response)
    {
        return new GetDeviceDontPropagateListReply(response);
    }
}