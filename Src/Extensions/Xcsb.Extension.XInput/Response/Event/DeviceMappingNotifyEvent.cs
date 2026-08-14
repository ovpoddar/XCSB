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

    public ref readonly DeviceMappingNotifyEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<DeviceMappingNotifyEvent>();
        if ((byte)result.ResponseHeader.Reply == (byte)XiInputEventType.DeviceMappingNotify || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}