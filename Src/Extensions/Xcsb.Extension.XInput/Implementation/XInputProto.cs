using System;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection;
using Xcsb.Connection.Handlers;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Models;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Contract;
using Xcsb.Connection.Response.Replies;
using Xcsb.Extension.XInput.Infrastructure;
using Xcsb.Extension.XInput.Infrastructure.ResponceProto;
using Xcsb.Extension.XInput.Infrastructure.VoidProto;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.TypeInfo;
using Xcsb.Extension.XInput.Models.Writers;
using Xcsb.Extension.XInput.Requests;
using Xcsb.Extension.XInput.Response.Event;
using Xcsb.Extension.XInput.Response.Replies;
using Xcsb.Extension.XInput.Response.Replies.Internals;
using Xcsb.Generators;
using Xcsb.Masks;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Implementation;

// https://xorg.freedesktop.org/archive/X11R7.7/doc/libXi/inputlib.pdf
[Implementation(ImplementationKind.Base | ImplementationKind.Checked| ImplementationKind.Unchecked, typeof(IVoidProto))]
[Implementation(ImplementationKind.Async | ImplementationKind.Return, typeof(IResponseProto))]
internal sealed partial class XInputProto : IXinputRequest
{
    private readonly QueryExtensionReply _response;
    private readonly IXExtensionInternal _extensionInternal;
    private readonly ISocketAccessor _socketAccessor;
    const int _minStackSupport = 512;

    public XInputProto(QueryExtensionReply response, IXExtensionInternal extensionInternal)
    {
        _response = response;
        _extensionInternal = extensionInternal;
        _socketAccessor = extensionInternal.Transport;
        Resister(extensionInternal);
    }

    static void Resister(IXExtensionInternal extension)
    {
        extension.RegisterX1Event<DeviceValuatorEvent>(XiInputEventType.DeviceValuator, XInputExtension.ExtensionName);
        extension.RegisterX1Event<DeviceKeyPressEvent>(XiInputEventType.DeviceKeyPress, XInputExtension.ExtensionName);
        extension.RegisterX1Event<DeviceKeyReleaseEvent>(XiInputEventType.DeviceKeyRelease, XInputExtension.ExtensionName);
        extension.RegisterX1Event<DeviceButtonPressEvent>(XiInputEventType.DeviceButtonPress, XInputExtension.ExtensionName);
        extension.RegisterX1Event<DeviceButtonReleaseEvent>(XiInputEventType.DeviceButtonRelease, XInputExtension.ExtensionName);
        extension.RegisterX1Event<DeviceMotionNotifyEvent>(XiInputEventType.DeviceMotionNotify, XInputExtension.ExtensionName);
        extension.RegisterX1Event<DeviceFocusInEvent>(XiInputEventType.DeviceFocusIn, XInputExtension.ExtensionName);
        extension.RegisterX1Event<DeviceFocusOutEvent>(XiInputEventType.DeviceFocusOut, XInputExtension.ExtensionName);
        extension.RegisterX1Event<ProximityInEvent>(XiInputEventType.ProximityIn, XInputExtension.ExtensionName);
        extension.RegisterX1Event<ProximityOutEvent>(XiInputEventType.ProximityOut, XInputExtension.ExtensionName);
        extension.RegisterX1Event<DeviceStateNotifyEvent>(XiInputEventType.DeviceStateNotify, XInputExtension.ExtensionName);
        extension.RegisterX1Event<DeviceMappingNotifyEvent>(XiInputEventType.DeviceMappingNotify, XInputExtension.ExtensionName);
        extension.RegisterX1Event<ChangeDeviceNotifyEvent>(XiInputEventType.ChangeDeviceNotify, XInputExtension.ExtensionName);
        extension.RegisterX1Event<DeviceKeyStateNotifyEvent>(XiInputEventType.DeviceKeyStateNotify, XInputExtension.ExtensionName);
        extension.RegisterX1Event<DeviceButtonStateNotifyEvent>(XiInputEventType.DeviceButtonStateNotify, XInputExtension.ExtensionName);
        extension.RegisterX1Event<DevicePresenceNotifyEvent>(XiInputEventType.DevicePresenceNotify, XInputExtension.ExtensionName);
        extension.RegisterX1Event<DevicePropertyNotifyEvent>(XiInputEventType.DevicePropertyNotify, XInputExtension.ExtensionName);

        extension.RegisterX2Event<DeviceChangedEvent>(XiInputEventType.DeviceChanged, XInputExtension.ExtensionName);
        extension.RegisterX2Event<KeyPressEvent>(XiInputEventType.KeyPress, XInputExtension.ExtensionName);
        extension.RegisterX2Event<KeyRelease>(XiInputEventType.KeyRelease, XInputExtension.ExtensionName);
        extension.RegisterX2Event<ButtonPressEvent>(XiInputEventType.ButtonPress, XInputExtension.ExtensionName);
        extension.RegisterX2Event<ButtonRelease>(XiInputEventType.ButtonRelease, XInputExtension.ExtensionName);
        extension.RegisterX2Event<Motion>(XiInputEventType.Motion, XInputExtension.ExtensionName);
        extension.RegisterX2Event<EnterEvent>(XiInputEventType.Enter, XInputExtension.ExtensionName);
        extension.RegisterX2Event<Leave>(XiInputEventType.Leave, XInputExtension.ExtensionName);
        extension.RegisterX2Event<FocusIn>(XiInputEventType.FocusIn, XInputExtension.ExtensionName);
        extension.RegisterX2Event<FocusOut>(XiInputEventType.FocusOut, XInputExtension.ExtensionName);
        extension.RegisterX2Event<HierarchyEvent>(XiInputEventType.Hierarchy, XInputExtension.ExtensionName);
        extension.RegisterX2Event<PropertyEvent>(XiInputEventType.Property, XInputExtension.ExtensionName);
        extension.RegisterX2Event<RawKeyPressEvent>(XiInputEventType.RawKeyPress, XInputExtension.ExtensionName);
        extension.RegisterX2Event<RawKeyRelease>(XiInputEventType.RawKeyRelease, XInputExtension.ExtensionName);
        extension.RegisterX2Event<RawButtonPressEvent>(XiInputEventType.RawButtonPress, XInputExtension.ExtensionName);
        extension.RegisterX2Event<RawButtonRelease>(XiInputEventType.RawButtonRelease, XInputExtension.ExtensionName);
        extension.RegisterX2Event<RawMotion>(XiInputEventType.RawMotion, XInputExtension.ExtensionName);
        extension.RegisterX2Event<TouchBeginEvent>(XiInputEventType.TouchBegin, XInputExtension.ExtensionName);
        extension.RegisterX2Event<TouchUpdate>(XiInputEventType.TouchUpdate, XInputExtension.ExtensionName);
        extension.RegisterX2Event<TouchEnd>(XiInputEventType.TouchEnd, XInputExtension.ExtensionName);
        extension.RegisterX2Event<TouchOwnershipEvent>(XiInputEventType.TouchOwnership, XInputExtension.ExtensionName);
        extension.RegisterX2Event<RawTouchBeginEvent>(XiInputEventType.RawTouchBegin, XInputExtension.ExtensionName);
        extension.RegisterX2Event<RawTouchUpdate>(XiInputEventType.RawTouchUpdate, XInputExtension.ExtensionName);
        extension.RegisterX2Event<RawTouchEnd>(XiInputEventType.RawTouchEnd, XInputExtension.ExtensionName);
        extension.RegisterX2Event<BarrierHitEvent>(XiInputEventType.BarrierHit, XInputExtension.ExtensionName);
        extension.RegisterX2Event<BarrierLeave>(XiInputEventType.BarrierLeave, XInputExtension.ExtensionName);
    }

    public ReplyLease<GetExtensionVersionReply> GetExtensionVersion(ReadOnlySpan<byte> name)
    {
        var cookie = GetExtensionVersionBase(name);
        var (result, mapping) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetExtensionVersionReply,GetExtensionVersionReply>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetExtensionVersionReply>(result, this._extensionInternal.Transport.SocketIn.BufferPool);
        
        var error = result.ToStruct<XResponse>();
        this._extensionInternal.Transport.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));

    }

    private ResponseProto CloseDeviceBase(byte deviceId)
    {
        var request = new CloseDeviceType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto SelectExtensionEventBase(uint window, ReadOnlySpan<uint> classes)
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

    private ResponseProto ChangeDeviceDontPropagateListBase(uint window, PropagateMode mode, ReadOnlySpan<uint> classes)
    {
        var request = new ChangeDeviceDontPropagateListType(this._response.MajorOpcode, window, mode,
            (ushort)classes.Length);
        Span<byte> scratchBuffer = stackalloc byte[request.Length * 4];
        scratchBuffer.WriteRequest(
            ref request,
            12,
            MemoryMarshal.Cast<uint, byte>(classes));
        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto UngrabDeviceBase(uint time, byte deviceId)
    {
        var request = new UngrabDeviceType(this._response.MajorOpcode, time, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto UngrabDeviceKeyBase(uint grabWindow, ModifierMask modifiers, byte modifierDevice, byte key,
        byte grabbedDevice)
    {
        var request = new UngrabDeviceKeyType(this._response.MajorOpcode, grabWindow, modifiers, modifierDevice, key,
            grabbedDevice);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto AllowDeviceEventsBase(uint time, DeviceInputMode mode, byte deviceId)
    {
        var request = new AllowDeviceEventsType(this._response.MajorOpcode, time, mode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto SetDeviceFocusBase(uint focus, uint time, InputFocusMode revertTo, byte deviceId)
    {
        var request = new SetDeviceFocusType(this._response.MajorOpcode, focus, time, revertTo, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto ChangeFeedbackControlBase<T>(FeedbackControlMask mask, byte deviceId, byte feedbackId,
        T feedback) where T : IFeedback
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

    private ResponseProto DeviceBellBase(byte deviceId, byte feedbackId, byte feedbackClass, sbyte percent)
    {
        var request = new DeviceBellType(this._response.MajorOpcode, deviceId, feedbackId, feedbackClass, percent);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto ChangeDevicePropertyBase<T>(ATOM property, ATOM type, byte deviceId, PropertyMode mode,
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

    private ResponseProto DeleteDevicePropertyBase(ATOM property, byte deviceId)
    {
        var request = new DeleteDevicePropertyType(this._response.MajorOpcode, property, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiChangeCursorBase(uint window, uint cursor, InputDevice deviceId)
    {
        var request = new XiChangeCursorType(this._response.MajorOpcode, window, cursor, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiChangeHierarchyBase(HierarchyChangeBuilder builder)
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

    private ResponseProto XiSetClientPointerBase(uint window, InputDevice deviceId)
    {
        var request = new XiSetClientPointerType(this._response.MajorOpcode, window, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiSelectEventsBase(uint window, EventMaskBuilder mask)
    {
        var request =
            new XiSelectEventsType(this._response.MajorOpcode, window, (ushort)mask.m_length, mask.m_data.Length);
        Span<byte> scratchBuffer = stackalloc byte[request.Length * 4];
        scratchBuffer.WriteRequest(
            ref request,
            12,
            MemoryMarshal.Cast<uint, byte>(mask.m_data));
        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiSetFocusBase(uint window, uint time, InputDevice deviceId)
    {
        var request = new XiSetFocusType(this._response.MajorOpcode, window, time, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiUngrabDeviceBase(uint time, InputDevice deviceId)
    {
        var request = new XiUngrabDeviceType(this._response.MajorOpcode, time, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiAllowEventsBase(uint time, InputDevice deviceId, EventMode eventMode, uint touchId,
        uint grabWindow)
    {
        var request = new XiAllowEventsType(this._response.MajorOpcode, time, deviceId, eventMode, touchId, grabWindow);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiDeletePropertyBase(InputDevice deviceId, ATOM property)
    {
        var request = new XiDeletePropertyType(this._response.MajorOpcode, deviceId, property);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiBarrierReleasePointerBase(ReadOnlySpan<BarrierReleasePointerInfo> barriers)
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

    private ResponseProto XiPassiveUngrabDeviceBase(uint grabWindow, uint detail, InputDevice deviceId,
        GrabType grabType,
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

    private ResponseProto XiChangePropertyBase<T>(InputDevice deviceId, PropertyMode mode, ATOM property, ATOM type,
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

    private ResponseProto ChangeDeviceKeyMappingBase(byte deviceId, byte firstKeycode, byte keysymsPerKeycode,
        byte keycodeCount, ReadOnlySpan<uint> keysyms)
    {
        var request = new ChangeDeviceKeyMappingType(this._response.MajorOpcode, deviceId, firstKeycode,
            keysymsPerKeycode, keycodeCount);
        var requestSize = request.Length * 4;
        Span<byte> scratchBuffer = stackalloc byte[requestSize];
        scratchBuffer.WriteRequest(
            ref request,
            16,
            MemoryMarshal.Cast<uint, byte>(keysyms));
        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GrabDeviceButtonBase(uint grabWindow, byte grabbedDevice, byte modifierDevice,
        ModifierMask modifiers, GrabMode thisDeviceMode, GrabMode otherDeviceMode, byte button, bool ownerEvents,
        ReadOnlySpan<uint> classes)
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

    private ResponseProto GrabDeviceKeyBase(uint grabWindow, ModifierMask modifiers, byte modifierDevice,
        byte grabbedDevice,
        byte key, GrabMode thisDeviceMode, GrabMode otherDeviceMode, bool ownerEvents, ReadOnlySpan<uint> classes)
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

    private ResponseProto UngrabDeviceButtonBase(uint grabWindow, ModifierMask modifiers, byte modifierDevice,
        byte button,
        byte grabbedDevice)
    {
        var request = new UngrabDeviceButtonType(this._response.MajorOpcode, grabWindow, modifiers, modifierDevice,
            button, grabbedDevice);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto SendExtensionEventBase(uint destination, byte deviceId, bool propagate,
        ReadOnlySpan<int> classes, ReadOnlySpan<InputEvents> events)
    {
        var request = new SendExtensionEventType(this._response.MajorOpcode, destination, deviceId, propagate,
            (byte)events.Length, (ushort)classes.Length);
        Span<byte> scratchBuffer = stackalloc byte[request.Length * 4];
        scratchBuffer.WriteRequest(
            ref request,
            16,
            MemoryMarshal.Cast<int, byte>(classes));
        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiWarpPointerBase(uint srcWin, uint dstWin, int srcX, int srcY, ushort srcWidth,
        ushort srcHeight, int dstX, int dstY, InputDevice deviceId)
    {
        var request = new XiWarpPointerType(this._response.MajorOpcode, srcWin, dstWin, srcX, srcY, srcWidth, srcHeight,
            dstX, dstY, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
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

    private ResponseProto ChangeDeviceControlBase(DeviceControl controlId, byte deviceId)
    {
        var request = new ChangeDeviceControlType(this._response.MajorOpcode, controlId, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto ListInputDevicesBase()
    {
        var request = new ListInputDevicesType(this._response.MajorOpcode);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto OpenDeviceBase(byte deviceId)
    {
        var request = new OpenDeviceType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto SetDeviceModeBase(byte deviceId, ValuatorMode mode)
    {
        var request = new SetDeviceModeType(this._response.MajorOpcode, deviceId, mode);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GetSelectedExtensionEventsBase(uint window)
    {
        var request = new GetSelectedExtensionEventsType(this._response.MajorOpcode, window);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GetDeviceDontPropagateListBase(uint window)
    {
        var request = new GetDeviceDontPropagateListType(this._response.MajorOpcode, window);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GetDeviceMotionEventsBase(uint start, uint stop, byte deviceId)
    {
        var request = new GetDeviceMotionEventsType(this._response.MajorOpcode, start, stop, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto ChangeKeyboardDeviceBase(byte deviceId)
    {
        var request = new ChangeKeyboardDeviceType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto ChangePointerDeviceBase(byte xAxis, byte yAxis, byte deviceId)
    {
        var request = new ChangePointerDeviceType(this._response.MajorOpcode, xAxis, yAxis, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GrabDeviceBase(uint grabWindow, uint time, GrabMode thisDeviceMode, GrabMode otherDeviceMode,
        bool ownerEvents, byte deviceId, ReadOnlySpan<uint> classes)
    {
        var request = new GrabDeviceType(this._response.MajorOpcode, grabWindow, time, (ushort)classes.Length,
            thisDeviceMode, otherDeviceMode, ownerEvents, deviceId);
        var requestSize = request.Length * 4;
        if (requestSize < _minStackSupport)
        {
            Span<byte> scratchBuffer = stackalloc byte[requestSize];
            scratchBuffer.WriteRequest(ref request, 20, MemoryMarshal.Cast<uint, byte>(classes));
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requestSize);
            scratchBuffer[..requestSize].WriteRequest(ref request, 20, MemoryMarshal.Cast<uint, byte>(classes));
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer[..requestSize], SocketFlags.None);
        }

        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GetDeviceFocusBase(byte deviceId)
    {
        var request = new GetDeviceFocusType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GetFeedbackControlBase(byte deviceId)
    {
        var request = new GetFeedbackControlType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GetDeviceKeyMappingBase(byte deviceId, byte firstKeycode, byte count)
    {
        var request = new GetDeviceKeyMappingType(this._response.MajorOpcode, deviceId, firstKeycode, count);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GetDeviceModifierMappingBase(byte deviceId)
    {
        var request = new GetDeviceModifierMappingType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto SetDeviceModifierMappingBase(byte deviceId, ReadOnlySpan<uint> keycodesPerModifier)
    {
        var request =
            new SetDeviceModifierMappingType(this._response.MajorOpcode, deviceId, (byte)keycodesPerModifier.Length);
        var requestSize = request.Length * 4;
        if (requestSize < _minStackSupport)
        {
            Span<byte> scratchBuffer = stackalloc byte[requestSize];
            scratchBuffer.WriteRequest(ref request, 8, MemoryMarshal.Cast<uint, byte>(keycodesPerModifier));
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requestSize);
            scratchBuffer[..requestSize]
                .WriteRequest(ref request, 8, MemoryMarshal.Cast<uint, byte>(keycodesPerModifier));
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer[..requestSize], SocketFlags.None);
        }

        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GetDeviceButtonMappingBase(byte deviceId)
    {
        var request = new GetDeviceButtonMappingType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto SetDeviceButtonMappingBase(byte deviceId, ReadOnlySpan<uint> map)
    {
        var request = new SetDeviceButtonMappingType(this._response.MajorOpcode, deviceId, (byte)map.Length);
        var requestSize = request.Length * 4;
        if (requestSize < _minStackSupport)
        {
            Span<byte> scratchBuffer = stackalloc byte[requestSize];
            scratchBuffer.WriteRequest(ref request, 8, MemoryMarshal.Cast<uint, byte>(map));
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requestSize);
            scratchBuffer[..requestSize].WriteRequest(ref request, 8, MemoryMarshal.Cast<uint, byte>(map));
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer[..requestSize], SocketFlags.None);
        }

        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto QueryDeviceStateBase(byte deviceId)
    {
        var request = new QueryDeviceStateType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto SetDeviceValuatorsBase(byte deviceId, byte firstValuator, ReadOnlySpan<uint> valuators)
    {
        var request =
            new SetDeviceValuatorsType(this._response.MajorOpcode, deviceId, firstValuator, (byte)valuators.Length);
        var requestSize = request.Length * 4;
        if (requestSize < _minStackSupport)
        {
            Span<byte> scratchBuffer = stackalloc byte[requestSize];
            scratchBuffer.WriteRequest(ref request, 20, MemoryMarshal.Cast<uint, byte>(valuators));
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requestSize);
            scratchBuffer[..requestSize].WriteRequest(ref request, 20, MemoryMarshal.Cast<uint, byte>(valuators));
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer[..requestSize], SocketFlags.None);
        }

        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GetDeviceControlBase(DeviceControl controlId, byte deviceId)
    {
        var request = new GetDeviceControlType(this._response.MajorOpcode, controlId, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto ListDevicePropertiesBase(byte deviceId)
    {
        var request = new ListDevicePropertiesType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GetDevicePropertyBase(ATOM property, ATOM type, uint offset, uint len, byte deviceId,
        bool delete)
    {
        var request = new GetDevicePropertyType(this._response.MajorOpcode, property, type, offset, len, deviceId,
            delete);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiQueryPointerBase(uint window, InputDevice deviceId)
    {
        var request = new XiQueryPointerType(this._response.MajorOpcode, window, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiGetClientPointerBase(uint window)
    {
        var request = new XiGetClientPointerType(this._response.MajorOpcode, window);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiQueryVersionBase(ushort majorVersion, ushort minorVersion)
    {
        var request = new XiQueryVersionType(this._response.MajorOpcode, majorVersion, minorVersion);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiQueryDeviceBase(InputDevice deviceId)
    {
        var request = new XiQueryDeviceType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiGetFocusBase(InputDevice deviceId)
    {
        var request = new XiGetFocusType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiGrabDeviceBase(uint window, uint time, uint cursor, InputDevice deviceId, GrabMode mode,
        GrabMode pairedDeviceMode, GrabOwner ownerEvents, ReadOnlySpan<uint> mask)
    {
        var request = new XiGrabDeviceType(this._response.MajorOpcode, window, time, cursor, deviceId, mode,
            pairedDeviceMode, ownerEvents, (ushort)mask.Length);
        var requestSize = request.Length * 4;
        if (requestSize < _minStackSupport)
        {
            Span<byte> scratchBuffer = stackalloc byte[requestSize];
            scratchBuffer.WriteRequest(ref request, 24, MemoryMarshal.Cast<uint, byte>(mask));
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requestSize);
            scratchBuffer[..requestSize].WriteRequest(ref request, 24, MemoryMarshal.Cast<uint, byte>(mask));
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer[..requestSize], SocketFlags.None);
        }

        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiPassiveGrabDeviceBase(uint time, uint grabWindow, uint cursor, uint detail,
        InputDevice deviceId, GrabType grabType, GrabMode22 grabMode, GrabMode pairedDeviceMode, GrabOwner ownerEvents,
        ReadOnlySpan<uint> mask, ReadOnlySpan<uint> modifiers)
    {
        var request = new XiPassiveGrabDeviceType(this._response.MajorOpcode, time, grabWindow, cursor, detail,
            deviceId,
            (ushort)modifiers.Length, (ushort)mask.Length, grabType, grabMode, pairedDeviceMode, ownerEvents);
        var maskCast = MemoryMarshal.Cast<uint, byte>(mask);
        var modifiersCast = MemoryMarshal.Cast<uint, byte>(modifiers);
        var requestSize = request.Length * 4;
        if (requestSize < _minStackSupport)
        {
            Span<byte> scratchBuffer = stackalloc byte[requestSize];
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(scratchBuffer), request);
            maskCast.CopyTo(scratchBuffer[32..]);
            modifiersCast.CopyTo(scratchBuffer[(32 + maskCast.Length)..]);
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requestSize);
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(scratchBuffer[..]), request);
            maskCast.CopyTo(scratchBuffer[32..]);
            modifiersCast.CopyTo(scratchBuffer[(32 + maskCast.Length)..]);
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer[..requestSize], SocketFlags.None);
        }

        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiListPropertiesBase(InputDevice deviceId)
    {
        var request = new XiListPropertiesType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiGetPropertyBase(InputDevice deviceId, byte delete, ATOM property, ATOM type, uint offset,
        uint len)
    {
        var request = new XiGetPropertyType(this._response.MajorOpcode, deviceId, delete, property, type, offset,
            len);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiGetSelectedEventsBase(uint window)
    {
        var request = new XiGetSelectedEventsType(this._response.MajorOpcode, window);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }
}