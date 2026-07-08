using System;
using System.Runtime.CompilerServices;
using Xcsb.Connection.Helpers;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct GetDeviceModifierMappingReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly byte ReplyType;
    public readonly byte[] Keymaps;

    public GetDeviceModifierMappingReply(Span<byte> result)
    {
        ref readonly var response = ref result.AsStruct<GetDeviceModifierMappingResponse>();
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        ReplyType = response.ResponseHeader.GetValue();
        if (response.KeycodesPerModifier == 0)
            Keymaps = Array.Empty<byte>();
        else
        {
            var responseLength = Unsafe.SizeOf<GetDeviceModifierMappingResponse>();
            Keymaps = result[responseLength..].ToArray();
        }
    }
}