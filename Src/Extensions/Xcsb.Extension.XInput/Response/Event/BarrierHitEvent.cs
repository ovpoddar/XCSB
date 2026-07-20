using System;
using System.Runtime.InteropServices;
using Xcsb.Connection;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct BarrierHitEvent : IXExtensionEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }

    public T Create<T>(Span<byte> data) where T : struct
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ButtonPressEvent : IXExtensionEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }

    public T Create<T>(Span<byte> data) where T : struct
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct DeviceChangedEvent : IXExtensionEvent
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

    public T Create<T>(Span<byte> data) where T : struct
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public unsafe struct DeviceKeyStateNotifyEvent : IXExtensionEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public fixed byte Keys[28];

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }

    public T Create<T>(Span<byte> data) where T : struct
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EnterEvent : IXExtensionEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }

    public T Create<T>(Span<byte> data) where T : struct
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GesturePinchBeginEvent : IXExtensionEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }

    public T Create<T>(Span<byte> data) where T : struct
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GestureSwipeBeginEvent : IXExtensionEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }

    public T Create<T>(Span<byte> data) where T : struct
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct HierarchyEvent : IXExtensionEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }

    public T Create<T>(Span<byte> data) where T : struct
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct KeyPressEvent : IXExtensionEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }

    public T Create<T>(Span<byte> data) where T : struct
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PropertyEvent : IXExtensionEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }

    public T Create<T>(Span<byte> data) where T : struct
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RawButtonPressEvent : IXExtensionEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }

    public T Create<T>(Span<byte> data) where T : struct
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RawKeyPressEvent : IXExtensionEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }

    public T Create<T>(Span<byte> data) where T : struct
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RawTouchBeginEvent : IXExtensionEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    private IXExtensionEvent _ixEventImplementation;

    public bool Verify()
    {
        return _ixEventImplementation.Verify();
    }

    public T Create<T>(Span<byte> data) where T : struct
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TouchBeginEvent : IXExtensionEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }

    public T Create<T>(Span<byte> data) where T : struct
    {
        throw new NotImplementedException();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TouchOwnershipEvent : IXExtensionEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }

    public T Create<T>(Span<byte> data) where T : struct
    {
        throw new NotImplementedException();
    }
}