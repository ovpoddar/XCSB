using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct DeviceMappingNotifyEvent : IXEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly byte RequestId;
    public readonly byte FirstKeyCode;
    public readonly byte Count;
    private readonly byte _pad;
    public readonly uint Time;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }
}