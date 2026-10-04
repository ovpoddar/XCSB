using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.TypeInfo;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct DevicePresenceNotifyEvent : IXEvent<DevicePresenceNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly DeviceChange Change;
    public readonly byte DeviceId;
    public readonly ushort Control;

    public DevicePresenceNotifyEvent FromBytes(byte[] response)
    {
        return response.AsSpan().ToStruct<DevicePresenceNotifyEvent>();
    }
}