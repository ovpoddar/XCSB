using System;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct XiQueryDeviceReply : IXReply<XiQueryDeviceReply, XiQueryDeviceResponse>
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly XIDeviceInfo[] DeviceInfos;

    public XiQueryDeviceReply(Span<byte> result)
    {
        ref readonly var response = ref result.AsStruct<XiQueryDeviceResponse>();
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        if (response.NumInfos == 0)
            DeviceInfos = Array.Empty<XIDeviceInfo>();
        else
        {
            DeviceInfos = new XIDeviceInfo[response.NumInfos];
            var index = 32;
            for (var i = 0; i < DeviceInfos.Length; i++)
                DeviceInfos[i] = XIDeviceInfo.Parse(result[index..], ref index);
        }
    }

    public bool Verify(in int sequence)
    {
        throw new NotImplementedException();
    }

    public XiQueryDeviceReply FromBytes(byte[] response)
    {
        return new XiQueryDeviceReply(response);
    }
}