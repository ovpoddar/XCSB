using System;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Replies;
using Xcsb.Extension.XInput.Infrastructure;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.Writers;
using Xcsb.Extension.XInput.Requests;
using Xcsb.Extension.XInput.Response.Replies;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Implementation;

internal sealed class XInputProto : IXinputRequest
{
    private readonly QueryExtensionReply _response;
    private readonly IXExtensionInternal _extensionInternal;
    const int _minStackSupport = 512;

    public XInputProto(QueryExtensionReply response, IXExtensionInternal extensionInternal)
    {
        _response = response;
        _extensionInternal = extensionInternal;
    }

    public GetExtensionVersionReply GetExtensionVersion(ReadOnlySpan<byte> name)
    {
        var cookie = GetExtensionVersionBase(name);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetExtensionVersionReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().ToStruct<GetExtensionVersionReply>();
    }

    private ResponseProto GetExtensionVersionBase(ReadOnlySpan<byte> name)
    {
        var request = new GetExtensionVersionType(this._response.MajorOpcode, (ushort)name.Length);
        var requestSize = request.Length * 4;
        if (requestSize < _minStackSupport)
        {
            Span<byte> scratchBuffer = stackalloc byte[requestSize];
            scratchBuffer.WriteRequest(ref request, 8, name);
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requestSize);
            scratchBuffer[..requestSize].WriteRequest(ref request, 8, name);
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer[..requestSize], SocketFlags.None);
        }

        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }


    public ResponseProto CloseDevice(byte deviceId)
    {
        var request = new CloseDeviceType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto SelectExtensionEvent(uint window, ReadOnlySpan<uint> classes)
    {
        var request = new SelectExtensionEventType(this._response.MajorOpcode, window, (ushort)classes.Length);
        Span<byte> scratchBuffer = stackalloc byte[request.Length * 4];
        scratchBuffer.WriteRequest(
            ref request,
            12,
            MemoryMarshal.Cast<uint, byte>(classes));
        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto ChangeDeviceDontPropagateList(uint window, byte mode, ReadOnlySpan<uint> classes)
    {
        var request =
            new ChangeDeviceDontPropagateListType(this._response.MajorOpcode, window, mode, (ushort)classes.Length);
        Span<byte> scratchBuffer = stackalloc byte[request.Length * 4];
        scratchBuffer.WriteRequest(
            ref request,
            12,
            MemoryMarshal.Cast<uint, byte>(classes));
        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto UngrabDevice(uint time, byte deviceId)
    {
        var request = new UngrabDeviceType(this._response.MajorOpcode, time, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto GrabDeviceKey(uint grabWindow, ushort modifiers, byte modifierDevice, byte grabbedDevice,
        byte key,
        byte thisDeviceMode, byte otherDeviceMode, byte ownerEvents, ReadOnlySpan<uint> classes)
    {
        var request = new GrabDeviceKeyType(this._response.MajorOpcode, grabWindow, modifiers, grabbedDevice,
            modifierDevice, key, thisDeviceMode, otherDeviceMode, ownerEvents, (ushort)classes.Length);
        Span<byte> scratchBuffer = stackalloc byte[request.Length * 4];
        scratchBuffer.WriteRequest(
            ref request,
            20,
            MemoryMarshal.Cast<uint, byte>(classes));
        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto UngrabDeviceKey(uint grabWindow, ushort modifiers, byte modifierDevice, byte key,
        byte grabbedDevice)
    {
        var request = new UngrabDeviceKeyType(this._response.MajorOpcode, grabWindow, modifiers, modifierDevice, key,
            grabbedDevice);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto GrabDeviceButton(uint grabWindow, byte grabbedDevice, byte modifierDevice, ushort modifiers,
        byte thisDeviceMode, byte otherDeviceMode, byte button, byte ownerEvents, ReadOnlySpan<uint> classes)
    {
        var request = new GrabDeviceButtonType(this._response.MajorOpcode, grabWindow, grabbedDevice, modifierDevice,
            modifiers, thisDeviceMode, otherDeviceMode, button, ownerEvents, (ushort)classes.Length);
        Span<byte> scratchBuffer = stackalloc byte[request.Length * 4];
        scratchBuffer.WriteRequest(
            ref request,
            20,
            MemoryMarshal.Cast<uint, byte>(classes));
        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto UngrabDeviceButton(uint grabWindow, ushort modifiers, byte modifierDevice, byte button,
        byte grabbedDevice)
    {
        var request = new UngrabDeviceButtonType(this._response.MajorOpcode, grabWindow, modifiers, modifierDevice,
            button, grabbedDevice);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto AllowDeviceEvents(uint time, byte mode, byte deviceId)
    {
        var request = new AllowDeviceEventsType(this._response.MajorOpcode, time, mode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto SetDeviceFocus(uint focus, uint time, byte revertTo, byte deviceId)
    {
        var request = new SetDeviceFocusType(this._response.MajorOpcode, focus, time, revertTo, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto ChangeFeedbackControl<T>(FeedbackControlMask mask, byte deviceId, byte feedbackId, T feedback)
        where T : IFeedback
    {
        var request = new ChangeFeedbackControlType(this._response.MajorOpcode, mask, deviceId, feedbackId,
            feedback.Length);
        var requestSize = request.Length * 4;
        Span<byte> scratchBuffer = stackalloc byte[requestSize];
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(scratchBuffer), request);
        if (feedback is not StringFeedback fb)
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(scratchBuffer[12..]), feedback);
        else
        {
            var feedbackHead = fb.m_feedback;
            scratchBuffer[12..].WriteRequest(
                ref feedbackHead,
                8,
                MemoryMarshal.Cast<uint, byte>(fb.m_keysyms)
            );
        }

        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto ChangeDeviceKeyMapping(byte deviceId, byte firstKeycode, byte keysymsPerKeycode, byte keycodeCount,
        ReadOnlySpan<uint> keysyms)
    {
        var request = new ChangeDeviceKeyMappingType(this._response.MajorOpcode, deviceId, firstKeycode, keysymsPerKeycode,
            keycodeCount);
        var requestSize = request.Length * 4;
        Span<byte> scratchBuffer = stackalloc byte[requestSize];
        scratchBuffer.WriteRequest(
            ref request,
            16,
            MemoryMarshal.Cast<uint, byte>(keysyms));
        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto DeviceBell(byte deviceId, byte feedbackId, byte feedbackClass, sbyte percent)
    {
        var request = new DeviceBellType(this._response.MajorOpcode, deviceId, feedbackId, feedbackClass, percent);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto ChangeDeviceProperty<T>(ATOM property, ATOM type, byte deviceId, PropertyMode mode,
        ReadOnlySpan<T> items) where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    {
        var size = Marshal.SizeOf<T>();
        if (size is not 1 and not 2 and not 4)
            throw new ArgumentException("type must be byte, sbyte, short, ushort, int, uint");
        var request = new ChangeDevicePropertyType(this._response.MajorOpcode, property, type, deviceId, size,
            mode, (ushort)items.Length);
        Span<byte> scratchBuffer = stackalloc byte[request.Length * 4];
        scratchBuffer.WriteRequest(
            ref request,
            16,
            MemoryMarshal.Cast<T, byte>(items));
        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto DeleteDeviceProperty(ATOM property, byte deviceId)
    {
        var request = new DeleteDevicePropertyType(this._response.MajorOpcode, property, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiWarpPointer(uint srcWin, uint dstWin, int srcX, int srcY, ushort srcWidth, ushort srcHeight,
        int dstX, int dstY, ushort deviceId)
    {
        var request = new XiWarpPointerType(this._response.MajorOpcode, srcWin, dstWin, srcX, srcY, srcWidth, srcHeight,
            dstX, dstY, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiChangeCursor(uint window, uint cursor, ushort deviceId)
    {
        var request = new XiChangeCursorType(this._response.MajorOpcode, window, cursor, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiChangeHierarchy(HierarchyChangeBuilder builder)
    {
        var request = new XiChangeHierarchyType(this._response.MajorOpcode, (byte)builder.m_length,
            builder.m_data.Length / 4);
        Span<byte> scratchBuffer = stackalloc byte[request.Length * 4];
        scratchBuffer.WriteRequest(
            ref request,
            8,
            builder.m_data);
        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiSetClientPointer(uint window, ushort deviceId)
    {
        var request = new XiSetClientPointerType(this._response.MajorOpcode, window, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiSelectEvents(uint window, EventMaskBuilder mask)
    {
        var request = new XiSelectEventsType(this._response.MajorOpcode, window, (ushort)mask.m_length);
        Span<byte> scratchBuffer = stackalloc byte[request.Length * 4];
        scratchBuffer.WriteRequest(
            ref request,
            12,
            MemoryMarshal.Cast<uint, byte>(mask.m_data));
        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiSetFocus(uint window, uint time, ushort deviceId)
    {
        var request = new XiSetFocusType(this._response.MajorOpcode, window, time, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiUngrabDevice(uint time, ushort deviceId)
    {
        var request = new XiUngrabDeviceType(this._response.MajorOpcode, time, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiAllowEvents(uint time, ushort deviceId, byte eventMode, uint touchId, uint grabWindow)
    {
        var request = new XiAllowEventsType(this._response.MajorOpcode, time, deviceId, eventMode, touchId, grabWindow);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiPassiveUngrabDevice(uint grabWindow, uint detail, InputDevice deviceId, GrabType grabType, 
        ReadOnlySpan<uint> modifiers)
    {
        var request = new XiPassiveUngrabDeviceType(this._response.MajorOpcode, grabWindow, detail, deviceId, 
            (ushort)modifiers.Length, grabType);
        Span<byte> scratchBuffer = stackalloc byte[request.Length * 4];
        scratchBuffer.WriteRequest(
            ref request,
            20,
            MemoryMarshal.Cast<uint, byte>(modifiers));
        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiChangeProperty<T>(ushort deviceId, PropertyMode mode, ATOM property, ATOM type,
        ReadOnlySpan<T> items) where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    {
        var size = Marshal.SizeOf<T>();
        if (size is not 1 and not 2 and not 4)
            throw new ArgumentException("type must be byte, sbyte, short, ushort, int, uint");
        var request = new XiChangePropertyType(this._response.MajorOpcode, deviceId, mode, (byte)size, property, type,
            (ushort)items.Length);
        Span<byte> scratchBuffer = stackalloc byte[request.Length * 4];
        scratchBuffer.WriteRequest(
            ref request,
            20,
            MemoryMarshal.Cast<T, byte>(items));
        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiDeleteProperty(ushort deviceId, ATOM property)
    {
        var request = new XiDeletePropertyType(this._response.MajorOpcode, deviceId, property);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiBarrierReleasePointer(ReadOnlySpan<BarrierReleasePointerInfo> barriers)
    {
        var request = new XiBarrierReleasePointerType(this._response.MajorOpcode, (uint)barriers.Length);
        Span<byte> scratchBuffer = stackalloc byte[request.Length * 4];
        scratchBuffer.WriteRequest(
            ref request,
            8,
            MemoryMarshal.Cast<BarrierReleasePointerInfo, byte>(barriers));
        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto SendExtensionEvent(uint destination, byte deviceId, byte propagate, byte numEvents,
        ReadOnlySpan<int> classes)
    {
        var request = new SendExtensionEventType(this._response.MajorOpcode, destination, deviceId, propagate,
            numEvents,
            (ushort)classes.Length);
        Span<byte> scratchBuffer = stackalloc byte[request.Length * 4];
        scratchBuffer.WriteRequest(
            ref request,
            16,
            MemoryMarshal.Cast<int, byte>(classes));
        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ChangeDeviceControlReply ChangeDeviceControl(ushort controlId, byte deviceId)
    {
        var cookie = ChangeDeviceControlBase(controlId, deviceId);
        var (result, error) = _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<ChangeDeviceControlReply>
            (cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.AsSpan().ToStruct<ChangeDeviceControlReply>();
    }

    private ResponseProto ChangeDeviceControlBase(ushort controlId, byte deviceId)
    {
        var request = new ChangeDeviceControlType(this._response.MajorOpcode, controlId, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }
}