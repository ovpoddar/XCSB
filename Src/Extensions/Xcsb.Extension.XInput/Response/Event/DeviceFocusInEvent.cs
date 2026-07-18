using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct DeviceFocusInEvent : IXEvent
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
}