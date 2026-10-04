using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.TypeInfo;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct DeviceMappingNotifyEvent : IXEvent<DeviceMappingNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly byte RequestId;
    public readonly byte FirstKeyCode;
    public readonly byte Count;
    private readonly byte _pad;
    public readonly uint Time;

    public DeviceMappingNotifyEvent FromBytes(Span<byte> response)
    {
        return response.ToStruct<DeviceMappingNotifyEvent>();
    }
}