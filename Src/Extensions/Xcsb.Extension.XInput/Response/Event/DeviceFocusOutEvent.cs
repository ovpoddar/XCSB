using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.TypeInfo;
using Xcsb.Models;
using NotifyDetail = Xcsb.Extension.XInput.Models.NotifyDetail;
using NotifyMode = Xcsb.Extension.XInput.Models.NotifyMode;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct DeviceFocusOutEvent : IXEvent<DeviceFocusOutEvent>
{
    public readonly ResponseHeader<ResponseType, NotifyDetail> ResponseHeader;
    public readonly uint Time;
    public readonly uint Window;
    public readonly NotifyMode Mode;
    public readonly byte DeviceId;

    public DeviceFocusOutEvent FromBytes(Span<byte> response)
    {
        return response.ToStruct<DeviceFocusOutEvent>();
    }
}