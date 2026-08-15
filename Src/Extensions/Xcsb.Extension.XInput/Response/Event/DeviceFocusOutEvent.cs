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

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }

    public ref readonly DeviceFocusOutEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<DeviceFocusOutEvent>();
        if ((byte)result.ResponseHeader.Reply == (byte)XiInputEventType.DeviceFocusOut || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}