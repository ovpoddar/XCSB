using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct BarrierHitEvent : IXEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ButtonPressEvent : IXEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct DeviceChangedEvent : IXEvent
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
    
    public bool Verify()
    {
        throw new System.NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public unsafe struct DeviceKeyStateNotifyEvent : IXEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public fixed byte Keys[28];

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EnterEvent : IXEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GesturePinchBeginEvent : IXEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GestureSwipeBeginEvent : IXEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct HierarchyEvent : IXEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct KeyPressEvent : IXEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PropertyEvent : IXEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RawButtonPressEvent : IXEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RawKeyPressEvent : IXEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RawTouchBeginEvent : IXEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    private IXEvent _ixEventImplementation;

    public bool Verify()
    {
        return _ixEventImplementation.Verify();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TouchBeginEvent : IXEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TouchOwnershipEvent : IXEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }
}