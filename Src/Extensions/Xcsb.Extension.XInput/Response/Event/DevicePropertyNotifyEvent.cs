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


    public ref readonly DevicePropertyNotifyEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<DevicePropertyNotifyEvent>();
        if ((byte)result.ResponseHeader.Reply == (byte)XiInputEventType.DevicePropertyNotify || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}