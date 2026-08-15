using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct DeviceChangedEvent : IXEvent<DeviceChangedEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint Length;
    public readonly ushort EventType;
    public readonly InputDevice DeviceId;
    public readonly uint Time;
    private readonly ushort _classLength;
    public readonly ushort SourceId;
    public readonly ChangeReason Reason;
    private fixed byte _pad[11];

    public readonly uint[] Classes;


    public ref readonly DeviceChangedEvent Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}