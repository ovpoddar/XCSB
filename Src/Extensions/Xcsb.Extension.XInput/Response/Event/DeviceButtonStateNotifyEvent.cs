using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.TypeInfo;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public unsafe struct DeviceButtonStateNotifyEvent : IXEvent<DeviceButtonStateNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public fixed byte Buttons[28];

    public DeviceButtonStateNotifyEvent FromBytes(byte[] response)
    {
        return response.AsSpan().ToStruct<DeviceButtonStateNotifyEvent>();
    }
}