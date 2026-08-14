using System;
using System.Runtime.InteropServices;
using Xcsb.Connection;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RawKeyRelease : IXEvent<RawKeyRelease>
{
    public ref readonly RawKeyRelease Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RawMotion : IXEvent<RawMotion>
{
    public ref readonly RawMotion Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TouchUpdate : IXEvent<TouchUpdate>
{
    public ref readonly TouchUpdate Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TouchEnd : IXEvent<TouchEnd>
{
    public ref readonly TouchEnd Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RawTouchUpdate : IXEvent<RawTouchUpdate>
{
    public ref readonly RawTouchUpdate Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RawTouchEnd : IXEvent<RawTouchEnd>
{
    public ref readonly RawTouchEnd Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct BarrierLeave : IXEvent<BarrierLeave>
{
    public ref readonly BarrierLeave Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct BarrierHitEvent : IXEvent<BarrierHitEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;


    public ref readonly BarrierHitEvent Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ButtonPressEvent : IXEvent<ButtonPressEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;


    public ref readonly ButtonPressEvent Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

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


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ButtonRelease: IXEvent<ButtonRelease>
{
    public ref readonly ButtonRelease Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Motion: IXEvent<Motion>
{
    public ref readonly Motion Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct KeyRelease: IXEvent<KeyRelease>
{
    public ref readonly KeyRelease Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RawButtonRelease: IXEvent<RawButtonRelease>
{
    public ref readonly RawButtonRelease Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TouchBeginEvent: IXEvent<TouchBeginEvent>
{
    public ref readonly TouchBeginEvent Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EnterEvent : IXEvent<EnterEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;


    public ref readonly EnterEvent Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Leave : IXEvent<Leave>
{
    public ref readonly Leave Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct FocusIn : IXEvent<FocusIn>
{
    public ref readonly FocusIn Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct FocusOut : IXEvent<FocusOut>
{
    public ref readonly FocusOut Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GesturePinchBeginEvent : IXEvent<GesturePinchBeginEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;


    public ref readonly GesturePinchBeginEvent Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GestureSwipeBeginEvent : IXEvent<GestureSwipeBeginEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;


    public ref readonly GestureSwipeBeginEvent Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct HierarchyEvent : IXEvent<HierarchyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;


    public ref readonly HierarchyEvent Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct KeyPressEvent : IXEvent<KeyPressEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;


    public ref readonly KeyPressEvent Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PropertyEvent : IXEvent<PropertyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;


    public ref readonly PropertyEvent Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RawButtonPressEvent : IXEvent<RawButtonPressEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;


    public ref readonly RawButtonPressEvent Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RawKeyPressEvent : IXEvent<RawKeyPressEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;


    public ref readonly RawKeyPressEvent Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RawTouchBeginEvent : IXEvent<RawTouchBeginEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;


    public ref readonly RawTouchBeginEvent Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TouchOwnershipEvent : IXEvent<TouchOwnershipEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;


    public ref readonly TouchOwnershipEvent Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}