using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.TypeInfo;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct DeviceFocusInEvent : IXEvent<DeviceFocusInEvent>
{
    public readonly ResponseHeader<ResponseType, NotifyDetail> ResponseHeader;
    public readonly uint Time;
    public readonly uint Window;
    public readonly NotifyMode Mode;
    public readonly byte DeviceId;


    public ref readonly DeviceFocusInEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<DeviceFocusInEvent>();
        if ((byte)result.ResponseHeader.Reply == (byte)XiInputEventType.DeviceFocusIn || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}