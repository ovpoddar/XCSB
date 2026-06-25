using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Xcsb.Connection.Helpers;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Response.Replies.Internals;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct ListInputDevicesReply
{
    public readonly byte DevicesLength;
    public readonly DeviceInfo[] DeviceInfos;

    internal ListInputDevicesReply(Span<byte> data)
    {
        ref readonly var context = ref data.AsStruct<ListInputDevicesResponse>();
        DevicesLength = context.DevicesLen;
        DeviceInfos = DevicesLength == 0 
            ? Array.Empty<DeviceInfo>() 
            : new DeviceInfo[DevicesLength];

        var readIndex = 32;
        for (var i = 0; i < DeviceInfos.Length; i++)
            DeviceInfos[i] = DeviceInfo.Read(data, ref readIndex);
    }
}