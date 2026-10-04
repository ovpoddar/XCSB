using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct XiPassiveGrabDeviceReply: IXReply<XiPassiveGrabDeviceReply, XiPassiveGrabDeviceResponse>
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public GrabModifierInfo[] Modifier;

    public XiPassiveGrabDeviceReply(Span<byte> result)
    {
        ref readonly var response = ref result.AsStruct<XiPassiveGrabDeviceResponse>();
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        if (response.NumModifiers == 0)
            Modifier = Array.Empty<GrabModifierInfo>();
        else
        {
            var responseSize = Unsafe.SizeOf<XiPassiveGrabDeviceResponse>();
            Modifier = MemoryMarshal.Cast<byte, GrabModifierInfo>(result[responseSize..]).ToArray();
        }
    }

    public bool Verify(in int sequence)
    {
        throw new NotImplementedException();
    }

    public XiPassiveGrabDeviceReply FromBytes(byte[] response)
    {
        return new XiPassiveGrabDeviceReply(response);
    }
}