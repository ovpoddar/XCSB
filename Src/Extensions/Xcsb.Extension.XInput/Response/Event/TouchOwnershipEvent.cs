using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TouchOwnershipEvent : IXEvent<TouchOwnershipEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint Length;
    public readonly ushort EventType;
    public readonly InputDevice DeviceId;
    public readonly uint Time;
    public readonly uint TouchId;
    public readonly uint Root;
    public readonly uint Event;
    public readonly uint Child;
    public readonly InputDevice SourceId;
    private readonly ushort _pad0;
    public readonly TouchOwnershipFlags Flags;

    public ref readonly TouchOwnershipEvent Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}