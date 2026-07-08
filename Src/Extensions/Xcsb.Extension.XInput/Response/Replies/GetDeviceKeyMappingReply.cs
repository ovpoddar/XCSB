using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct GetDeviceKeyMappingReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly byte ReplyType;
    public readonly uint[] Keysyms;

    public GetDeviceKeyMappingReply(Span<byte> result)
    {
        ref readonly var response = ref result.AsStruct<GetDeviceKeyMappingResponse>();
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        ReplyType = response.ResponseHeader.GetValue();
        
        if (response.Length == 0)
            Keysyms = Array.Empty<uint>();
        else
        {
            var requestLength = Unsafe.SizeOf<GetDeviceKeyMappingResponse>();
            Keysyms = MemoryMarshal.Cast<byte, uint>(result[requestLength..]).ToArray();
        }
    }
}