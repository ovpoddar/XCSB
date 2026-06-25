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

    private ResponseProto ListInputDevicesBase()
    {
        var request = new ListInputDevicesType(this._response.MajorOpcode);
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