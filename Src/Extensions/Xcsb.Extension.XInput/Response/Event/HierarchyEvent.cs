using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct HierarchyEvent : IXEvent<HierarchyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint Length;
    public readonly ushort EventType;
    public readonly InputDevice DeviceId;
    public readonly uint Time;
    public readonly HierarchyMask Flags;
    private readonly ushort _informationLength;
    private fixed byte _pad[10];
    public readonly HierarchyInfo[] Informations;

    public ref readonly HierarchyEvent Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}