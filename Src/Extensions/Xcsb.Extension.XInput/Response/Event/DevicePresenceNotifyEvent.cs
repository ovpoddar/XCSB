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

    public ref readonly DevicePresenceNotifyEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<DevicePresenceNotifyEvent>();
        if ((byte)result.ResponseHeader.Reply == (byte)XiInputEventType.DevicePresenceNotify || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}