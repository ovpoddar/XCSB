using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public unsafe struct DevicePropertyNotifyEvent : IXEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint Time;
    public readonly ATOM Property;
    private fixed byte _pad[19];
    public readonly byte DeviceId;
    
    public bool Verify()
    {
        throw new System.NotImplementedException();
    }
}