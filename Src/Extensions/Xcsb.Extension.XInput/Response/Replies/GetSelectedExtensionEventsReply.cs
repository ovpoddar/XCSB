using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct GetSelectedExtensionEventsReply: IXReply<GetSelectedExtensionEventsReply, GetSelectedExtensionEventsResponse>
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly byte ReplyType;
    public readonly uint[] ThisClasses;
    public readonly uint[] AllClasses;

    internal GetSelectedExtensionEventsReply(Span<byte> result)
    {
        ref readonly var response = ref result.AsStruct<GetSelectedExtensionEventsResponse>();
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        ReplyType = response.ResponseHeader.GetValue();

        var responseLength = Unsafe.SizeOf<GetSelectedExtensionEventsResponse>();
        if (response.NumThisClasses == 0)
            ThisClasses = Array.Empty<uint>();
        else
        {
            var classSize = response.NumThisClasses * Unsafe.SizeOf<uint>();
            ThisClasses = MemoryMarshal.Cast<byte, uint>(result[responseLength..classSize]).ToArray();
            responseLength += classSize;
        }

        if (response.NumAllClasses == 0)
            AllClasses = Array.Empty<uint>();
        else
        {
            var classSize = response.NumAllClasses * Unsafe.SizeOf<uint>();
            AllClasses = MemoryMarshal.Cast<byte, uint>(result[responseLength..classSize]).ToArray();
        }
    }

    public bool Verify(in int sequence)
    {
        throw new NotImplementedException();
    }

    public GetSelectedExtensionEventsReply FromBytes(Span<byte> response)
    {
        return new GetSelectedExtensionEventsReply(response);
    }
}