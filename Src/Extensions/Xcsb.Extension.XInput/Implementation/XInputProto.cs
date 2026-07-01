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
using Xcsb.Extension.XInput.Infrastructure.ResponceProto;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.Writers;
using Xcsb.Extension.XInput.Requests;
using Xcsb.Extension.XInput.Response.Replies;
using Xcsb.Extension.XInput.Response.Replies.Internals;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Implementation;

// https://xorg.freedesktop.org/archive/X11R7.7/doc/libXi/inputlib.pdf
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

    public ChangeDeviceControlReply ChangeDeviceControl(ushort controlId, byte deviceId)
    {
        var cookie = ChangeDeviceControlBase(controlId, deviceId);
        var (result, error) = _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<ChangeDeviceControlReply>
            (cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.AsSpan().ToStruct<ChangeDeviceControlReply>();
    }

    public ListInputDevicesReply ListInputDevices()
    {
        var cookie = ListInputDevicesBase();
        var (result, error) =
            _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<ListInputDevicesResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new ListInputDevicesReply(result);
    }

    public OpenDeviceReply OpenDevice(byte deviceId)
    {
        var cookie = OpenDeviceBase(deviceId);
        var (result, error) = _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<OpenDeviceResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new OpenDeviceReply(result);
    }

    public SetDeviceModeReply SetDeviceMode(byte deviceId, byte mode)
    {
        var cookie = SetDeviceModeBase(deviceId, mode);
        var (result, error) = _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<SetDeviceModeReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.AsSpan().AsStruct<SetDeviceModeReply>();
    }

    public GetSelectedExtensionEventsReply GetSelectedExtensionEvents(uint window)
    {
        var cookie = GetSelectedExtensionEventsBase(window);
        var (result, error) =
            _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetSelectedExtensionEventsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetSelectedExtensionEventsReply(result);
    }

    public GetDeviceDontPropagateListReply GetDeviceDontPropagateList(uint window)
    {
        var cookie = GetDeviceDontPropagateListBase(window);
        var (result, error) = _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDeviceDontPropagateListResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetDeviceDontPropagateListReply(result);
    }

    public GetDeviceMotionEventsReply GetDeviceMotionEvents(uint start, uint stop, byte deviceId)
    {
        var cookie = GetDeviceMotionEventsBase(start, stop, deviceId);
        var (result, error) =
            _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDeviceMotionEventsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetDeviceMotionEventsReply(result);
    }

    public ChangeKeyboardDeviceReply ChangeKeyboardDevice(byte deviceId)
    {
        var cookie = ChangeKeyboardDeviceBase(deviceId);
        var (result, error) =
            _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<ChangeKeyboardDeviceReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.AsSpan().ToStruct<ChangeKeyboardDeviceReply>();
    }

    public ChangePointerDeviceReply ChangePointerDevice(byte xAxis, byte yAxis, byte deviceId)
    {
        var cookie = ChangePointerDeviceBase(xAxis, yAxis, deviceId);
        var (result, error) =_extensionInternal.Transport.SocketIn.ReceivedResponseSpan<ChangePointerDeviceReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.AsSpan().ToStruct<ChangePointerDeviceReply>();
    }

    public GrabDeviceReply GrabDevice(uint grabWindow, uint time, ushort numClasses, byte thisDeviceMode,
        byte otherDeviceMode, byte ownerEvents, byte deviceId)
    {
        throw new NotImplementedException();

        return result!.AsSpan().ToStruct<GrabDeviceReply>();
    }

    public GetDeviceFocusReply GetDeviceFocus(byte deviceId)
    {
        var cookie = GetDeviceFocusBase(deviceId);
        var (result, error) =
            _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDeviceFocusReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.AsSpan().ToStruct<GetDeviceFocusReply>();
    }

    public GetFeedbackControlReply GetFeedbackControl(byte deviceId)
    {
        var cookie = GetFeedbackControlBase(deviceId);
        var (result, error) = _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetFeedbackControlResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetFeedbackControlReply(result);
    }

    public GetDeviceKeyMappingReply GetDeviceKeyMapping(byte deviceId, byte firstKeycode, byte count)
    {
        var cookie = GetDeviceKeyMappingBase(deviceId, firstKeycode, count);
        var (result, error) = _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDeviceKeyMappingResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetDeviceKeyMappingReply(result);
    }

    public GetDeviceModifierMappingReply GetDeviceModifierMapping(byte deviceId)
    {
        var cookie = GetDeviceModifierMappingBase(deviceId);
        var (result, error) = _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDeviceModifierMappingResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetDeviceModifierMappingReply(result);
    }

    public SetDeviceModifierMappingReply SetDeviceModifierMapping(byte deviceId, byte keycodesPerModifier)
    {
        var cookie = SetDeviceModifierMappingBase(deviceId, keycodesPerModifier);
        var (result, error) =
            _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<SetDeviceModifierMappingReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.AsSpan().ToStruct<SetDeviceModifierMappingReply>();
    }

    public GetDeviceButtonMappingReply GetDeviceButtonMapping(byte deviceId)
    {
        var cookie = GetDeviceButtonMappingBase(deviceId);
        var (result, error) =
            _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDeviceButtonMappingResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetDeviceButtonMappingReply(result);
    }

    public SetDeviceButtonMappingReply SetDeviceButtonMapping(byte deviceId, byte mapSize)
    {
        var cookie = SetDeviceButtonMappingBase(deviceId, mapSize);
        var (result, error) = _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<SetDeviceButtonMappingReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.AsSpan().ToStruct<SetDeviceButtonMappingReply>();
    }

    public QueryDeviceStateReply QueryDeviceState(byte deviceId)
    {
        var cookie = QueryDeviceStateBase(deviceId);
        var (result, error) =
            _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<QueryDeviceStateResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new QueryDeviceStateReply(result);
    }

    public SetDeviceValuatorsReply SetDeviceValuators(byte deviceId, byte firstValuator, byte numValuators)
    {
        var cookie = SetDeviceValuatorsBase(deviceId, firstValuator, numValuators);
        var (result, error) = _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<SetDeviceValuatorsReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.AsSpan().ToStruct<SetDeviceValuatorsReply>();
    }

    public GetDeviceControlReply GetDeviceControl(ushort controlId, byte deviceId)
    {
        var cookie = GetDeviceControlBase(controlId, deviceId);
        var (result, error) = _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDeviceControlResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetDeviceControlReply(result);
    }

    public ListDevicePropertiesReply ListDeviceProperties(byte deviceId)
    {
        var cookie = ListDevicePropertiesBase(deviceId);
        var (result, error) = _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<ListDevicePropertiesReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.AsSpan().ToStruct<ListDevicePropertiesReply>();
    }

    public GetDevicePropertyReply GetDeviceProperty(ATOM property, ATOM type, uint offset, uint len, byte deviceId,
        byte delete)
    {
        var cookie = GetDevicePropertyBase(property, type, offset, len, deviceId, delete);
        var (result, error) = _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDevicePropertyReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.AsSpan().ToStruct<GetDevicePropertyReply>();
    }

    public XiQueryPointerReply XiQueryPointer(uint window, InputDevice deviceId)
    {
        var cookie = XiQueryPointerBase(window, deviceId);
        var (result, error) = _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiQueryPointerReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.AsSpan().ToStruct<XiQueryPointerReply>();
    }

    public XiGetClientPointerReply XiGetClientPointer(uint window)
    {
        var cookie = XiGetClientPointerBase(window);
        var (result, error) = _extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiGetClientPointerReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.AsSpan().ToStruct<XiGetClientPointerReply>();
    }

    public XiQueryVersionReply XiQueryVersion(ushort majorVersion, ushort minorVersion)
    {
        var cookie = XiQueryVersionBase(majorVersion, minorVersion);
    }

    public XiQueryDeviceReply XiQueryDevice(InputDevice deviceId)
    {
        var cookie = XiQueryDeviceBase(deviceId);
    }

    public XiGetFocusReply XiGetFocus(InputDevice deviceId)
    {
        var cookie = XiGetFocusBase(deviceId);
    }

    public XiGrabDeviceReply XiGrabDevice(uint window, uint time, uint cursor, InputDevice deviceId, byte mode,
        byte pairedDeviceMode, byte ownerEvents, byte pad0, ushort maskLen)
    {
        throw new NotImplementedException();
    }

    public XiPassiveGrabDeviceReply XiPassiveGrabDevice(uint time, uint grabWindow, uint cursor, uint detail,
        InputDevice deviceId,
        ushort numModifiers, ushort maskLen, byte grabType, byte grabMode, byte pairedDeviceMode, byte ownerEvents)
    {
        throw new NotImplementedException();
    }

    public XiListPropertiesReply XiListProperties(InputDevice deviceId)
    {
        var cookie = XiListPropertiesBase(deviceId);
    }

    public XiGetPropertyReply XiGetProperty(InputDevice deviceId, byte delete, byte pad0, ATOM property, ATOM type,
        uint offset, uint len)
    {
        throw new NotImplementedException();
    }

    public XiGetSelectedEventsReply XiGetSelectedEvents(uint window)
    {
        var cookie = XiGetSelectedEventsBase(window);
    }

    public ResponseProto CloseDevice(byte deviceId) =>
        CloseDeviceBase(deviceId);

    public ResponseProto SelectExtensionEvent(uint window, ReadOnlySpan<uint> classes) =>
        SelectExtensionEventBase(window, classes);

    public ResponseProto ChangeDeviceDontPropagateList(uint window, byte mode, ReadOnlySpan<uint> classes) =>
        ChangeDeviceDontPropagateListBase(window, mode, classes);

    public ResponseProto UngrabDevice(uint time, byte deviceId) =>
        UngrabDeviceBase(time, deviceId);

    public ResponseProto GrabDeviceKey(uint grabWindow, ushort modifiers, byte modifierDevice, byte grabbedDevice,
        byte key,
        byte thisDeviceMode, byte otherDeviceMode, byte ownerEvents, ReadOnlySpan<uint> classes) =>
        GrabDeviceKeyBase(grabWindow, modifiers, modifierDevice, grabbedDevice, key, thisDeviceMode, otherDeviceMode,
            ownerEvents, classes);

    public ResponseProto UngrabDeviceKey(uint grabWindow, ushort modifiers, byte modifierDevice, byte key,
        byte grabbedDevice) =>
        UngrabDeviceKeyBase(grabWindow, modifiers, modifierDevice, key, grabbedDevice);

    public ResponseProto GrabDeviceButton(uint grabWindow, byte grabbedDevice, byte modifierDevice, ushort modifiers,
        byte thisDeviceMode, byte otherDeviceMode, byte button, byte ownerEvents, ReadOnlySpan<uint> classes) =>
        GrabDeviceButtonBase(grabWindow, grabbedDevice, modifierDevice, modifiers, thisDeviceMode, otherDeviceMode,
            button, ownerEvents, classes);

    public ResponseProto UngrabDeviceButton(uint grabWindow, ushort modifiers, byte modifierDevice, byte button,
        byte grabbedDevice) =>
        UngrabDeviceButtonBase(grabWindow, modifiers, modifierDevice, button, grabbedDevice);

    public ResponseProto AllowDeviceEvents(uint time, byte mode, byte deviceId) =>
        AllowDeviceEventsBase(time, mode, deviceId);

    public ResponseProto SetDeviceFocus(uint focus, uint time, byte revertTo, byte deviceId) =>
        SetDeviceFocusBase(focus, time, revertTo, deviceId);

    public ResponseProto ChangeFeedbackControl<T>(FeedbackControlMask mask, byte deviceId, byte feedbackId, T feedback)
        where T : IFeedback =>
        ChangeFeedbackControlBase(mask, deviceId, feedbackId, feedback);

    public ResponseProto ChangeDeviceKeyMapping(byte deviceId, byte firstKeycode, byte keysymsPerKeycode,
        byte keycodeCount, ReadOnlySpan<uint> keysyms) =>
        ChangeDeviceKeyMappingBase(deviceId, firstKeycode, keysymsPerKeycode, keycodeCount, keysyms);

    public ResponseProto DeviceBell(byte deviceId, byte feedbackId, byte feedbackClass, sbyte percent) =>
        DeviceBellBase(deviceId, feedbackId, feedbackClass, percent);

    public ResponseProto ChangeDeviceProperty<T>(ATOM property, ATOM type, byte deviceId, PropertyMode mode,
        ReadOnlySpan<T> items) where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
        => ChangeDevicePropertyBase(property, type, deviceId, mode, items);

    public ResponseProto DeleteDeviceProperty(ATOM property, byte deviceId) =>
        DeleteDevicePropertyBase(property, deviceId);

    public ResponseProto XiWarpPointer(uint srcWin, uint dstWin, int srcX, int srcY, ushort srcWidth, ushort srcHeight,
        int dstX, int dstY, InputDevice deviceId) =>
        XiWarpPointerBase(srcWin, dstWin, srcX, srcY, srcWidth, srcHeight, dstX, dstY, deviceId);

    public ResponseProto XiChangeCursor(uint window, uint cursor, InputDevice deviceId) =>
        XiChangeCursorBase(window, cursor, deviceId);

    public ResponseProto XiChangeHierarchy(HierarchyChangeBuilder builder) =>
        XiChangeHierarchyBase(builder);

    public ResponseProto XiSetClientPointer(uint window, InputDevice deviceId) =>
        XiSetClientPointerBase(window, deviceId);

    public ResponseProto XiSelectEvents(uint window, EventMaskBuilder mask) =>
        XiSelectEventsBase(window, mask);

    public ResponseProto XiSetFocus(uint window, uint time, InputDevice deviceId) =>
        XiSetFocusBase(window, time, deviceId);

    public ResponseProto XiUngrabDevice(uint time, InputDevice deviceId) =>
        XiUngrabDeviceBase(time, deviceId);

    public ResponseProto XiAllowEvents(uint time, InputDevice deviceId, byte eventMode, uint touchId,
        uint grabWindow) => XiAllowEventsBase(time, deviceId, eventMode, touchId, grabWindow);

    public ResponseProto XiPassiveUngrabDevice(uint grabWindow, uint detail, InputDevice deviceId, GrabType grabType,
        ReadOnlySpan<uint> modifiers) =>
        XiPassiveUngrabDeviceBase(grabWindow, detail, deviceId, grabType, modifiers);

    public ResponseProto XiChangeProperty<T>(InputDevice deviceId, PropertyMode mode, ATOM property, ATOM type,
        ReadOnlySpan<T> items) where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
        => XiChangePropertyBase(deviceId, mode, property, type, items);

    public ResponseProto XiDeleteProperty(InputDevice deviceId, ATOM property) =>
        XiDeletePropertyBase(deviceId, property);

    public ResponseProto XiBarrierReleasePointer(ReadOnlySpan<BarrierReleasePointerInfo> barriers) =>
        XiBarrierReleasePointerBase(barriers);

    public ResponseProto SendExtensionEvent(uint destination, byte deviceId, byte propagate, byte numEvents,
        ReadOnlySpan<int> classes) =>
        SendExtensionEventBase(destination, deviceId, propagate, numEvents, classes);

    public void CloseDeviceChecked(byte deviceId)
    {
        var cookie = CloseDeviceBase(deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void SelectExtensionEventChecked(uint window, ReadOnlySpan<uint> classes)
    {
        var cookie = SelectExtensionEventBase(window, classes);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeDeviceDontPropagateListChecked(uint window, byte mode, ReadOnlySpan<uint> classes)
    {
        var cookie = ChangeDeviceDontPropagateListBase(window, mode, classes);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void UngrabDeviceChecked(uint time, byte deviceId)
    {
        var cookie = UngrabDeviceBase(time, deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void GrabDeviceKeyChecked(uint grabWindow, ushort modifiers, byte modifierDevice, byte grabbedDevice,
        byte key, byte thisDeviceMode, byte otherDeviceMode, byte ownerEvents, ReadOnlySpan<uint> classes)
    {
        var cookie = GrabDeviceKeyBase(grabWindow, modifiers, modifierDevice, grabbedDevice, key, thisDeviceMode,
            otherDeviceMode, ownerEvents, classes);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void UngrabDeviceKeyChecked(uint grabWindow, ushort modifiers, byte modifierDevice, byte key,
        byte grabbedDevice)
    {
        var cookie = UngrabDeviceKeyBase(grabWindow, modifiers, modifierDevice, key, grabbedDevice);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void GrabDeviceButtonChecked(uint grabWindow, byte grabbedDevice, byte modifierDevice, ushort modifiers,
        byte thisDeviceMode, byte otherDeviceMode, byte button, byte ownerEvents, ReadOnlySpan<uint> classes)
    {
        var cookie = GrabDeviceButtonBase(grabWindow, grabbedDevice, modifierDevice, modifiers, thisDeviceMode,
            otherDeviceMode, button, ownerEvents, classes);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void UngrabDeviceButtonChecked(uint grabWindow, ushort modifiers, byte modifierDevice, byte button,
        byte grabbedDevice)
    {
        var cookie = UngrabDeviceButtonBase(grabWindow, modifiers, modifierDevice, button, grabbedDevice);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void AllowDeviceEventsChecked(uint time, byte mode, byte deviceId)
    {
        var cookie = AllowDeviceEventsBase(time, mode, deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetDeviceFocusChecked(uint focus, uint time, byte revertTo, byte deviceId)
    {
        var cookie = SetDeviceFocusBase(focus, time, revertTo, deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeFeedbackControlChecked<T>(FeedbackControlMask mask, byte deviceId, byte feedbackId, T feedback)
        where T : IFeedback
    {
        var cookie = ChangeFeedbackControlBase(mask, deviceId, feedbackId, feedback);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeDeviceKeyMappingChecked(byte deviceId, byte firstKeycode, byte keysymsPerKeycode,
        byte keycodeCount, ReadOnlySpan<uint> keysyms)
    {
        var cookie = ChangeDeviceKeyMappingBase(deviceId, firstKeycode, keysymsPerKeycode, keycodeCount, keysyms);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void DeviceBellChecked(byte deviceId, byte feedbackId, byte feedbackClass, sbyte percent)
    {
        var cookie = DeviceBellBase(deviceId, feedbackId, feedbackClass, percent);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeDevicePropertyChecked<T>(ATOM property, ATOM type, byte deviceId, PropertyMode mode,
        ReadOnlySpan<T> items) where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    {
        var cookie = ChangeDevicePropertyBase(property, type, deviceId, mode, items);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void DeleteDevicePropertyChecked(ATOM property, byte deviceId)
    {
        var cookie = DeleteDevicePropertyBase(property, deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void XiWarpPointerChecked(uint srcWin, uint dstWin, int srcX, int srcY, ushort srcWidth, ushort srcHeight,
        int dstX, int dstY, InputDevice deviceId)
    {
        var cookie = XiWarpPointerBase(srcWin, dstWin, srcX, srcY, srcWidth, srcHeight, dstX, dstY, deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void XiChangeCursorChecked(uint window, uint cursor, InputDevice deviceId)
    {
        var cookie = XiChangeCursorBase(window, cursor, deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void XiChangeHierarchyChecked(HierarchyChangeBuilder builder)
    {
        var cookie = XiChangeHierarchyBase(builder);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void XiSetClientPointerChecked(uint window, InputDevice deviceId)
    {
        var cookie = XiSetClientPointerBase(window, deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void XiSelectEventsChecked(uint window, EventMaskBuilder mask)
    {
        var cookie = XiSelectEventsBase(window, mask);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void XiSetFocusChecked(uint window, uint time, InputDevice deviceId)
    {
        var cookie = XiSetFocusBase(window, time, deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void XiUngrabDeviceChecked(uint time, InputDevice deviceId)
    {
        var cookie = XiUngrabDeviceBase(time, deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void XiAllowEventsChecked(uint time, InputDevice deviceId, byte eventMode, uint touchId, uint grabWindow)
    {
        var cookie = XiAllowEventsBase(time, deviceId, eventMode, touchId, grabWindow);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void XiPassiveUngrabDeviceChecked(uint grabWindow, uint detail, InputDevice deviceId, GrabType grabType,
        ReadOnlySpan<uint> modifiers)
    {
        var cookie = XiPassiveUngrabDeviceBase(grabWindow, detail, deviceId, grabType, modifiers);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void XiChangePropertyChecked<T>(InputDevice deviceId, PropertyMode mode, ATOM property, ATOM type,
        ReadOnlySpan<T> items) where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    {
        var cookie = XiChangePropertyBase(deviceId, mode, property, type, items);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void XiDeletePropertyChecked(InputDevice deviceId, ATOM property)
    {
        var cookie = XiDeletePropertyBase(deviceId, property);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void XiBarrierReleasePointerChecked(ReadOnlySpan<BarrierReleasePointerInfo> barriers)
    {
        var cookie = XiBarrierReleasePointerBase(barriers);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void SendExtensionEventChecked(uint destination, byte deviceId, byte propagate, byte numEvents,
        ReadOnlySpan<int> classes)
    {
        var cookie = SendExtensionEventBase(destination, deviceId, propagate, numEvents, classes);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, true);
    }

    public void CloseDeviceUnchecked(byte deviceId)
    {
        var cookie = CloseDeviceBase(deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void SelectExtensionEventUnchecked(uint window, ReadOnlySpan<uint> classes)
    {
        var cookie = SelectExtensionEventBase(window, classes);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangeDeviceDontPropagateListUnchecked(uint window, byte mode, ReadOnlySpan<uint> classes)
    {
        var cookie = ChangeDeviceDontPropagateListBase(window, mode, classes);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void UngrabDeviceUnchecked(uint time, byte deviceId)
    {
        var cookie = UngrabDeviceBase(time, deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void GrabDeviceKeyUnchecked(uint grabWindow, ushort modifiers, byte modifierDevice, byte grabbedDevice,
        byte key, byte thisDeviceMode, byte otherDeviceMode, byte ownerEvents, ReadOnlySpan<uint> classes)
    {
        var cookie = GrabDeviceKeyBase(grabWindow, modifiers, modifierDevice, grabbedDevice, key, thisDeviceMode,
            otherDeviceMode, ownerEvents, classes);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void UngrabDeviceKeyUnchecked(uint grabWindow, ushort modifiers, byte modifierDevice, byte key,
        byte grabbedDevice)
    {
        var cookie = UngrabDeviceKeyBase(grabWindow, modifiers, modifierDevice, key, grabbedDevice);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void GrabDeviceButtonUnchecked(uint grabWindow, byte grabbedDevice, byte modifierDevice, ushort modifiers,
        byte thisDeviceMode, byte otherDeviceMode, byte button, byte ownerEvents, ReadOnlySpan<uint> classes)
    {
        var cookie = GrabDeviceButtonBase(grabWindow, grabbedDevice, modifierDevice, modifiers, thisDeviceMode,
            otherDeviceMode, button, ownerEvents, classes);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void UngrabDeviceButtonUnchecked(uint grabWindow, ushort modifiers, byte modifierDevice, byte button,
        byte grabbedDevice)
    {
        var cookie = UngrabDeviceButtonBase(grabWindow, modifiers, modifierDevice, button, grabbedDevice);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void AllowDeviceEventsUnchecked(uint time, byte mode, byte deviceId)
    {
        var cookie = AllowDeviceEventsBase(time, mode, deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void SetDeviceFocusUnchecked(uint focus, uint time, byte revertTo, byte deviceId)
    {
        var cookie = SetDeviceFocusBase(focus, time, revertTo, deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangeFeedbackControlUnchecked<T>(FeedbackControlMask mask, byte deviceId, byte feedbackId, T feedback)
        where T : IFeedback
    {
        var cookie = ChangeFeedbackControlBase(mask, deviceId, feedbackId, feedback);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangeDeviceKeyMappingUnchecked(byte deviceId, byte firstKeycode, byte keysymsPerKeycode,
        byte keycodeCount, ReadOnlySpan<uint> keysyms)
    {
        var cookie = ChangeDeviceKeyMappingBase(deviceId, firstKeycode, keysymsPerKeycode, keycodeCount, keysyms);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void DeviceBellUnchecked(byte deviceId, byte feedbackId, byte feedbackClass, sbyte percent)
    {
        var cookie = DeviceBellBase(deviceId, feedbackId, feedbackClass, percent);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangeDevicePropertyUnchecked<T>(ATOM property, ATOM type, byte deviceId, PropertyMode mode,
        ReadOnlySpan<T> items) where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    {
        var cookie = ChangeDevicePropertyBase(property, type, deviceId, mode, items);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void DeleteDevicePropertyUnchecked(ATOM property, byte deviceId)
    {
        var cookie = DeleteDevicePropertyBase(property, deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void XiWarpPointerUnchecked(uint srcWin, uint dstWin, int srcX, int srcY, ushort srcWidth, ushort srcHeight,
        int dstX, int dstY, InputDevice deviceId)
    {
        var cookie = XiWarpPointerBase(srcWin, dstWin, srcX, srcY, srcWidth, srcHeight, dstX, dstY, deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void XiChangeCursorUnchecked(uint window, uint cursor, InputDevice deviceId)
    {
        var cookie = XiChangeCursorBase(window, cursor, deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void XiChangeHierarchyUnchecked(HierarchyChangeBuilder builder)
    {
        var cookie = XiChangeHierarchyBase(builder);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void XiSetClientPointerUnchecked(uint window, InputDevice deviceId)
    {
        var cookie = XiSetClientPointerBase(window, deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void XiSelectEventsUnchecked(uint window, EventMaskBuilder mask)
    {
        var cookie = XiSelectEventsBase(window, mask);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void XiSetFocusUnchecked(uint window, uint time, InputDevice deviceId)
    {
        var cookie = XiSetFocusBase(window, time, deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void XiUngrabDeviceUnchecked(uint time, InputDevice deviceId)
    {
        var cookie = XiUngrabDeviceBase(time, deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void XiAllowEventsUnchecked(uint time, InputDevice deviceId, byte eventMode, uint touchId, uint grabWindow)
    {
        var cookie = XiAllowEventsBase(time, deviceId, eventMode, touchId, grabWindow);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void XiPassiveUngrabDeviceUnchecked(uint grabWindow, uint detail, InputDevice deviceId, GrabType grabType,
        ReadOnlySpan<uint> modifiers)
    {
        var cookie = XiPassiveUngrabDeviceBase(grabWindow, detail, deviceId, grabType, modifiers);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void XiChangePropertyUnchecked<T>(InputDevice deviceId, PropertyMode mode, ATOM property, ATOM type,
        ReadOnlySpan<T> items) where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    {
        var cookie = XiChangePropertyBase(deviceId, mode, property, type, items);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void XiDeletePropertyUnchecked(InputDevice deviceId, ATOM property)
    {
        var cookie = XiDeletePropertyBase(deviceId, property);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void XiBarrierReleasePointerUnchecked(ReadOnlySpan<BarrierReleasePointerInfo> barriers)
    {
        var cookie = XiBarrierReleasePointerBase(barriers);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void SendExtensionEventUnchecked(uint destination, byte deviceId, byte propagate, byte numEvents,
        ReadOnlySpan<int> classes)
    {
        var cookie = SendExtensionEventBase(destination, deviceId, propagate, numEvents, classes);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    private ResponseProto OpenDeviceBase(byte deviceId)
    {
        var request = new OpenDeviceType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto SetDeviceModeBase(byte deviceId, byte mode)
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

    private ResponseProto GrabDeviceBase(uint grabWindow, uint time, ushort numClasses, byte thisDeviceMode,
        byte otherDeviceMode,
        byte ownerEvents, byte deviceId)
    {
        throw new NotImplementedException();
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

    private ResponseProto SetDeviceModifierMappingBase(byte deviceId, byte keycodesPerModifier)
    {
        throw new NotImplementedException();
    }

    private ResponseProto GetDeviceButtonMappingBase(byte deviceId)
    {
        var request = new GetDeviceButtonMappingType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto SetDeviceButtonMappingBase(byte deviceId, byte mapSize)
    {
        throw new NotImplementedException();
    }

    private ResponseProto QueryDeviceStateBase(byte deviceId)
    {
        var request = new QueryDeviceStateType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto SetDeviceValuatorsBase(byte deviceId, byte firstValuator, byte numValuators)
    {
        throw new NotImplementedException();
    }

    private ResponseProto GetDeviceControlBase(ushort controlId, byte deviceId)
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
        byte delete)
    {
        var request =
            new GetDevicePropertyType(this._response.MajorOpcode, property, type, offset, len, deviceId, delete);
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
        throw new NotImplementedException();
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

    private ResponseProto XiGrabDeviceBase(uint window, uint time, uint cursor, InputDevice deviceId, byte mode,
        byte pairedDeviceMode,
        byte ownerEvents, byte pad0, ushort maskLen)
    {
        throw new NotImplementedException();
    }

    private ResponseProto XiPassiveGrabDeviceBase(uint time, uint grabWindow, uint cursor, uint detail,
        InputDevice deviceId,
        ushort numModifiers, ushort maskLen, byte grabType, byte grabMode, byte pairedDeviceMode, byte ownerEvents)
    {
        throw new NotImplementedException();
    }

    private ResponseProto XiListPropertiesBase(InputDevice deviceId)
    {
        var request = new XiListPropertiesType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiGetPropertyBase(InputDevice deviceId, byte delete, byte pad0, ATOM property, ATOM type,
        uint offset,
        uint len)
    {
        throw new NotImplementedException();
    }

    private ResponseProto XiGetSelectedEventsBase(uint window)
    {
        var request = new XiGetSelectedEventsType(this._response.MajorOpcode, window);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }


    private ResponseProto ListInputDevicesBase()
    {
        var request = new ListInputDevicesType(this._response.MajorOpcode);
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


    private ResponseProto ChangeDeviceControlBase(ushort controlId, byte deviceId)
    {
        var request = new ChangeDeviceControlType(this._response.MajorOpcode, controlId, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
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

    private ResponseProto ChangeDeviceDontPropagateListBase(uint window, byte mode, ReadOnlySpan<uint> classes)
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

    private ResponseProto UngrabDeviceBase(uint time, byte deviceId)
    {
        var request = new UngrabDeviceType(this._response.MajorOpcode, time, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GrabDeviceKeyBase(uint grabWindow, ushort modifiers, byte modifierDevice, byte grabbedDevice,
        byte key, byte thisDeviceMode, byte otherDeviceMode, byte ownerEvents, ReadOnlySpan<uint> classes)
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

    private ResponseProto UngrabDeviceKeyBase(uint grabWindow, ushort modifiers, byte modifierDevice, byte key,
        byte grabbedDevice)
    {
        var request = new UngrabDeviceKeyType(this._response.MajorOpcode, grabWindow, modifiers, modifierDevice, key,
            grabbedDevice);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GrabDeviceButtonBase(uint grabWindow, byte grabbedDevice, byte modifierDevice,
        ushort modifiers, byte thisDeviceMode, byte otherDeviceMode, byte button, byte ownerEvents,
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

    private ResponseProto UngrabDeviceButtonBase(uint grabWindow, ushort modifiers, byte modifierDevice, byte button,
        byte grabbedDevice)
    {
        var request = new UngrabDeviceButtonType(this._response.MajorOpcode, grabWindow, modifiers, modifierDevice,
            button, grabbedDevice);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto AllowDeviceEventsBase(uint time, byte mode, byte deviceId)
    {
        var request = new AllowDeviceEventsType(this._response.MajorOpcode, time, mode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto SetDeviceFocusBase(uint focus, uint time, byte revertTo, byte deviceId)
    {
        var request = new SetDeviceFocusType(this._response.MajorOpcode, focus, time, revertTo, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto ChangeFeedbackControlBase<T>(FeedbackControlMask mask, byte deviceId, byte feedbackId,
        T feedback)
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

    private ResponseProto ChangeDeviceKeyMappingBase(byte deviceId, byte firstKeycode, byte keysymsPerKeycode,
        byte keycodeCount,
        ReadOnlySpan<uint> keysyms)
    {
        var request = new ChangeDeviceKeyMappingType(this._response.MajorOpcode, deviceId, firstKeycode,
            keysymsPerKeycode,
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

    private ResponseProto XiWarpPointerBase(uint srcWin, uint dstWin, int srcX, int srcY, ushort srcWidth,
        ushort srcHeight, int dstX, int dstY, InputDevice deviceId)
    {
        var request = new XiWarpPointerType(this._response.MajorOpcode, srcWin, dstWin, srcX, srcY, srcWidth, srcHeight,
            dstX, dstY, deviceId);
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

    private ResponseProto XiAllowEventsBase(uint time, InputDevice deviceId, byte eventMode, uint touchId,
        uint grabWindow)
    {
        var request = new XiAllowEventsType(this._response.MajorOpcode, time, deviceId, eventMode, touchId, grabWindow);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiPassiveUngrabDeviceBase(uint grabWindow, uint detail, InputDevice deviceId,
        GrabType grabType, ReadOnlySpan<uint> modifiers)
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

    private ResponseProto SendExtensionEventBase(uint destination, byte deviceId, byte propagate, byte numEvents,
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
}