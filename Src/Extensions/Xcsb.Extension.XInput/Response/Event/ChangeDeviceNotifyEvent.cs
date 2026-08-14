using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.TypeInfo;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ChangeDeviceNotifyEvent : IXEvent<ChangeDeviceNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint Time;
    public readonly ChangeDevice Request;

    public ref readonly ChangeDeviceNotifyEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<ChangeDeviceNotifyEvent>();
        if ((byte)result.ResponseHeader.Reply == (byte)XiInputEventType.ChangeDeviceNotify || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}