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

    public ref readonly DeviceKeyStateNotifyEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<DeviceKeyStateNotifyEvent>();
        if ((byte)result.ResponseHeader.Reply == (byte)XiInputEventType.DeviceKeyStateNotify || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}