using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.TypeInfo;

namespace Xcsb.Extension.XInput.Response.Event;


[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public unsafe struct DeviceKeyStateNotifyEvent : IXEvent<DeviceKeyStateNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public fixed byte Keys[28];

    public DeviceKeyStateNotifyEvent FromBytes(Span<byte> response)
    {
        return response.ToStruct<DeviceKeyStateNotifyEvent>();
    }
}