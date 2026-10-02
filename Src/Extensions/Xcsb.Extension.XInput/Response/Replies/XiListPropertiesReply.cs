using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct XiListPropertiesReply: IXReply<XiListPropertiesReply, XiListPropertiesResponse>
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly ATOM[] Properties;

    internal XiListPropertiesReply(Span<byte> result)
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

    public bool Verify(in int sequence)
    {
        throw new NotImplementedException();
    }

    public XiListPropertiesReply FromBytes(Span<byte> response)
    {
        return new XiListPropertiesReply(response);
    }
}