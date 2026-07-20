using System;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection;
using Xcsb.Connection.Handlers;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Replies;
using Xcsb.Extension.XInput.Infrastructure;
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
[CheckedImplementation(typeof(IVoidProto))]
[UncheckedImplementation(typeof(IVoidProto))]
[BaseImplementation(typeof(IVoidProto))]
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
        Resister(response.MajorOpcode, extensionInternal);
    }

    static void Resister(byte responseMajorOpcode, IXExtensionInternal extension)
    {
        extension.RegisterX1Event<DeviceValuatorEvent>(XiInputEventType.DeviceValuator);
        extension.RegisterX1Event<DeviceKeyPressEvent>(XiInputEventType. DeviceKeyPress);
        extension.RegisterX1Event<DeviceKeyReleaseEvent>(XiInputEventType. DeviceKeyRelease);
        extension.RegisterX1Event<DeviceButtonPressEvent>(XiInputEventType. DeviceButtonPress);
        extension.RegisterX1Event<DeviceButtonReleaseEvent>(XiInputEventType. DeviceButtonRelease);
        extension.RegisterX1Event<DeviceMotionNotifyEvent>(XiInputEventType. DeviceMotionNotify);
        extension.RegisterX1Event<DeviceFocusInEvent>(XiInputEventType. DeviceFocusIn);
        extension.RegisterX1Event<DeviceFocusOutEvent>(XiInputEventType. DeviceFocusOut);
        extension.RegisterX1Event<ProximityInEvent>(XiInputEventType. ProximityIn);
        extension.RegisterX1Event<ProximityOutEvent>(XiInputEventType. ProximityOut);
        extension.RegisterX1Event<DeviceStateNotifyEvent>(XiInputEventType. DeviceStateNotify);
        extension.RegisterX1Event<DeviceMappingNotifyEvent>(XiInputEventType. DeviceMappingNotify);
        extension.RegisterX1Event<ChangeDeviceNotifyEvent>(XiInputEventType. ChangeDeviceNotify);
        extension.RegisterX1Event<DeviceKeyStateNotifyEvent>(XiInputEventType. DeviceKeyStateNotify);
        extension.RegisterX1Event<DeviceButtonStateNotifyEvent>(XiInputEventType. DeviceButtonStateNotify);
        extension.RegisterX1Event<DevicePresenceNotifyEvent>(XiInputEventType. DevicePresenceNotify);
        extension.RegisterX1Event<DevicePropertyNotifyEvent>(XiInputEventType. DevicePropertyNotify);

        // extension.RegisterX2Event<DeviceChangedEvent>(responseMajorOpcode, XiInputEventType.DeviceChanged);
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

    public ChangeDeviceControlReply ChangeDeviceControl(DeviceControl controlId, byte deviceId)
    {
        var cookie = ChangeDeviceControlBase(controlId, deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<ChangeDeviceControlReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<ChangeDeviceControlReply>();
    }

    public ListInputDevicesReply ListInputDevices()
    {
        var cookie = ListInputDevicesBase();
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<ListInputDevicesResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new ListInputDevicesReply(result);
    }

    public OpenDeviceReply OpenDevice(byte deviceId)
    {
        var cookie = OpenDeviceBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<OpenDeviceReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<OpenDeviceReply>();
    }

    public SetDeviceModeReply SetDeviceMode(byte deviceId, ValuatorMode mode)
    {
        var cookie = SetDeviceModeBase(deviceId, mode);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<SetDeviceModeReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<SetDeviceModeReply>();
    }

    public GetSelectedExtensionEventsReply GetSelectedExtensionEvents(uint window)
    {
        var cookie = GetSelectedExtensionEventsBase(window);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn
                .ReceivedResponseSpan<GetSelectedExtensionEventsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetSelectedExtensionEventsReply(result);
    }

    public GetDeviceDontPropagateListReply GetDeviceDontPropagateList(uint window)
    {
        var cookie = GetDeviceDontPropagateListBase(window);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn
                .ReceivedResponseSpan<GetDeviceDontPropagateListResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetDeviceDontPropagateListReply(result);
    }

    public GetDeviceMotionEventsReply GetDeviceMotionEvents(uint start, uint stop, byte deviceId)
    {
        var cookie = GetDeviceMotionEventsBase(start, stop, deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDeviceMotionEventsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetDeviceMotionEventsReply(result);
    }

    public ChangeKeyboardDeviceReply ChangeKeyboardDevice(byte deviceId)
    {
        var cookie = ChangeKeyboardDeviceBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<ChangeKeyboardDeviceReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<ChangeKeyboardDeviceReply>();
    }

    public ChangePointerDeviceReply ChangePointerDevice(byte xAxis, byte yAxis, byte deviceId)
    {
        var cookie = ChangePointerDeviceBase(xAxis, yAxis, deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<ChangePointerDeviceReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<ChangePointerDeviceReply>();
    }

    public GrabDeviceReply GrabDevice(uint grabWindow, uint time, GrabMode thisDeviceMode, GrabMode otherDeviceMode,
        bool ownerEvents, byte deviceId, ReadOnlySpan<uint> classes)
    {
        var cookie = GrabDeviceBase(grabWindow, time, thisDeviceMode, otherDeviceMode, ownerEvents, deviceId,
            classes);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GrabDeviceReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<GrabDeviceReply>();
    }

    public GetDeviceFocusReply GetDeviceFocus(byte deviceId)
    {
        var cookie = GetDeviceFocusBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDeviceFocusReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<GetDeviceFocusReply>();
    }

    public GetFeedbackControlReply GetFeedbackControl(byte deviceId)
    {
        var cookie = GetFeedbackControlBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetFeedbackControlResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetFeedbackControlReply(result);
    }

    public GetDeviceKeyMappingReply GetDeviceKeyMapping(byte deviceId, byte firstKeycode, byte count)
    {
        var cookie = GetDeviceKeyMappingBase(deviceId, firstKeycode, count);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDeviceKeyMappingResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetDeviceKeyMappingReply(result);
    }

    public GetDeviceModifierMappingReply GetDeviceModifierMapping(byte deviceId)
    {
        var cookie = GetDeviceModifierMappingBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn
                .ReceivedResponseSpan<GetDeviceModifierMappingResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetDeviceModifierMappingReply(result);
    }

    public SetDeviceModifierMappingReply SetDeviceModifierMapping(byte deviceId, ReadOnlySpan<uint> keycodesPerModifier)
    {
        var cookie = SetDeviceModifierMappingBase(deviceId, keycodesPerModifier);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<SetDeviceModifierMappingReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<SetDeviceModifierMappingReply>();
    }

    public GetDeviceButtonMappingReply GetDeviceButtonMapping(byte deviceId)
    {
        var cookie = GetDeviceButtonMappingBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDeviceButtonMappingReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<GetDeviceButtonMappingReply>();
    }

    public SetDeviceButtonMappingReply SetDeviceButtonMapping(byte deviceId, ReadOnlySpan<uint> map)
    {
        var cookie = SetDeviceButtonMappingBase(deviceId, map);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<SetDeviceButtonMappingReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<SetDeviceButtonMappingReply>();
    }

    public QueryDeviceStateReply QueryDeviceState(byte deviceId)
    {
        var cookie = QueryDeviceStateBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<QueryDeviceStateResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new QueryDeviceStateReply(result);
    }

    public SetDeviceValuatorsReply SetDeviceValuators(byte deviceId, byte firstValuator, ReadOnlySpan<uint> valuators)
    {
        var cookie = SetDeviceValuatorsBase(deviceId, firstValuator, valuators);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<SetDeviceValuatorsReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<SetDeviceValuatorsReply>();
    }

    public GetDeviceControlReply GetDeviceControl(DeviceControl controlId, byte deviceId)
    {
        var cookie = GetDeviceControlBase(controlId, deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDeviceControlReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<GetDeviceControlReply>();
    }

    public ListDevicePropertiesReply ListDeviceProperties(byte deviceId)
    {
        var cookie = ListDevicePropertiesBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<ListDevicePropertiesResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new ListDevicePropertiesReply(result);
    }

    public GetDevicePropertyReply GetDeviceProperty(ATOM property, ATOM type, uint offset, uint len, byte deviceId,
        bool delete)
    {
        var cookie = GetDevicePropertyBase(property, type, offset, len, deviceId, delete);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDevicePropertyReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<GetDevicePropertyReply>();
    }

    public XiQueryPointerReply XiQueryPointer(uint window, InputDevice deviceId)
    {
        var cookie = XiQueryPointerBase(window, deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiQueryPointerResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new XiQueryPointerReply(result);
    }

    public XiGetClientPointerReply XiGetClientPointer(uint window)
    {
        var cookie = XiGetClientPointerBase(window);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiGetClientPointerReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<XiGetClientPointerReply>();
    }

    public XiQueryVersionReply XiQueryVersion(ushort majorVersion, ushort minorVersion)
    {
        var cookie = XiQueryVersionBase(majorVersion, minorVersion);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiQueryVersionReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<XiQueryVersionReply>();
    }

    public XiQueryDeviceReply XiQueryDevice(InputDevice deviceId)
    {
        var cookie = XiQueryDeviceBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiQueryDeviceResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new XiQueryDeviceReply(result);
    }

    public XiGetFocusReply XiGetFocus(InputDevice deviceId)
    {
        var cookie = XiGetFocusBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiGetFocusReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<XiGetFocusReply>();
    }

    public XiGrabDeviceReply XiGrabDevice(uint window, uint time, uint cursor, InputDevice deviceId, GrabMode mode,
        GrabMode pairedDeviceMode, GrabOwner ownerEvents, ReadOnlySpan<uint> classes)
    {
        var cookie = XiGrabDeviceBase(window, time, cursor, deviceId, mode, pairedDeviceMode, ownerEvents, classes);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiGrabDeviceReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<XiGrabDeviceReply>();
    }

    public XiPassiveGrabDeviceReply XiPassiveGrabDevice(uint time, uint grabWindow, uint cursor, uint detail,
        InputDevice deviceId, GrabType grabType, GrabMode22 grabMode, GrabMode pairedDeviceMode, GrabOwner ownerEvents,
        ReadOnlySpan<uint> mask, ReadOnlySpan<uint> modifiers)
    {
        var cookie = XiPassiveGrabDeviceBase(time, grabWindow, cursor, detail, deviceId, grabType, grabMode,
            pairedDeviceMode, ownerEvents, mask, modifiers);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiPassiveGrabDeviceResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new XiPassiveGrabDeviceReply(result);
    }

    public XiListPropertiesReply XiListProperties(InputDevice deviceId)
    {
        var cookie = XiListPropertiesBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiListPropertiesResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new XiListPropertiesReply(result);
    }

    public XiGetPropertyReply XiGetProperty(InputDevice deviceId, byte delete, ATOM property, ATOM type, uint offset,
        uint len)
    {
        var cookie = XiGetPropertyBase(deviceId, delete, property, type, offset, len);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiGetPropertyReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<XiGetPropertyReply>();
    }

    public XiGetSelectedEventsReply XiGetSelectedEvents(uint window)
    {
        var cookie = XiGetSelectedEventsBase(window);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiGetSelectedEventsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new XiGetSelectedEventsReply(result);
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
        var request = new XiSelectEventsType(this._response.MajorOpcode, window, (ushort)mask.m_length);
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
        var request= new SetDeviceModifierMappingType(this._response.MajorOpcode, deviceId, (byte)keycodesPerModifier.Length);
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
            scratchBuffer[..requestSize].WriteRequest(ref request, 8, MemoryMarshal.Cast<uint, byte>(keycodesPerModifier));
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
        var request = new SetDeviceValuatorsType(this._response.MajorOpcode, deviceId, firstValuator, (byte)valuators.Length);
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
        var request = new XiPassiveGrabDeviceType(this._response.MajorOpcode, time, grabWindow, cursor, detail, deviceId,
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