using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct PropertyEvent : IXEvent<PropertyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint Length;
    public readonly ushort EventType;
    public readonly InputDevice DeviceId;
    public readonly uint Time;
    public readonly Xcsb.Models.ATOM Property;
    public readonly PropertyFlag What;
    private fixed byte _pad0[11];


    public ref readonly PropertyEvent Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}