using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Extension.XInput.Models;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct XiListPropertiesReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly ATOM[] Properties;

    public XiListPropertiesReply(Span<byte> result)
    {
        ref readonly var response = ref result.AsStruct<XiListPropertiesResponse>();
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        if (response.NumProperties == 0)
            Properties = Array.Empty<ATOM>();
        else
        {
            var responseSize = Unsafe.SizeOf<XiListPropertiesResponse>();
            Properties = MemoryMarshal.Cast<byte, ATOM>(result[responseSize..]).ToArray();
        }
    }
}