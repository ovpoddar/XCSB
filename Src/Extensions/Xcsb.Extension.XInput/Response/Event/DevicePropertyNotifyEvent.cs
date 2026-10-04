using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.TypeInfo;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public unsafe struct DevicePropertyNotifyEvent : IXEvent<DevicePropertyNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint Time;
    public readonly ATOM Property;
    private fixed byte _pad[19];
    public readonly byte DeviceId;


    public DevicePropertyNotifyEvent FromBytes(byte[] response)
    {
        return response.AsSpan().ToStruct<DevicePropertyNotifyEvent>();
    }
}