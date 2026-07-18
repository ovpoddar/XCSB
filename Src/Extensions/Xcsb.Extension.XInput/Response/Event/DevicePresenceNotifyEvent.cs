using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct DevicePresenceNotifyEvent : IXEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly DeviceChange Change;
    public readonly byte DeviceId;
    public readonly ushort Control;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }
}