using System;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Replies;
using Xcsb.Extension.XInput.Infrastructure.VoidProto;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.Writers;
using Xcsb.Extension.XInput.Requests;
using Xcsb.Masks;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Implementation;

internal sealed partial class XInputProto
{
    public ResponseProto CloseDevice(byte deviceId) =>
        CloseDeviceBase(deviceId);

    public ResponseProto SelectExtensionEvent(uint window, ReadOnlySpan<uint> classes) =>
        SelectExtensionEventBase(window, classes);

    public ResponseProto ChangeDeviceDontPropagateList(uint window, PropagateMode mode, ReadOnlySpan<uint> classes) =>
        ChangeDeviceDontPropagateListBase(window, mode, classes);

    public ResponseProto UngrabDevice(uint time, byte deviceId) =>
        UngrabDeviceBase(time, deviceId);

    public ResponseProto GrabDeviceKey(uint grabWindow, ModifierMask modifiers, byte modifierDevice, byte grabbedDevice,
        byte key,
        GrabMode thisDeviceMode, GrabMode otherDeviceMode, bool ownerEvents, ReadOnlySpan<uint> classes) =>
        GrabDeviceKeyBase(grabWindow, modifiers, modifierDevice, grabbedDevice, key, thisDeviceMode,
            otherDeviceMode, ownerEvents, classes);


    public ResponseProto UngrabDeviceKey(uint grabWindow, ModifierMask modifiers, byte modifierDevice, byte key,
        byte grabbedDevice) =>
        UngrabDeviceKeyBase(grabWindow, modifiers, modifierDevice, key, grabbedDevice);

    public ResponseProto GrabDeviceButton(uint grabWindow, byte grabbedDevice, byte modifierDevice, ModifierMask modifiers,
        GrabMode thisDeviceMode, GrabMode otherDeviceMode, byte button, bool ownerEvents, ReadOnlySpan<uint> classes) =>
        GrabDeviceButtonBase(grabWindow, grabbedDevice, modifierDevice, modifiers, thisDeviceMode, otherDeviceMode,
            button, ownerEvents, classes);

    public ResponseProto UngrabDeviceButton(uint grabWindow, ModifierMask modifiers, byte modifierDevice, byte button,
        byte grabbedDevice) =>
        UngrabDeviceButtonBase(grabWindow, modifiers, modifierDevice, button, grabbedDevice);

    public ResponseProto AllowDeviceEvents(uint time, DeviceInputMode mode, byte deviceId) =>
        AllowDeviceEventsBase(time, mode, deviceId);

    public ResponseProto SetDeviceFocus(uint focus, uint time, InputFocusMode revertTo, byte deviceId) =>
        SetDeviceFocusBase(focus, time, revertTo, deviceId);

    public ResponseProto ChangeFeedbackControl<T>(FeedbackControlMask mask, byte deviceId, byte feedbackId, T feedback)
        where T : IFeedback =>
        ChangeFeedbackControlBase(mask, deviceId, feedbackId, feedback);

    public ResponseProto ChangeDeviceKeyMapping(byte deviceId, byte firstKeycode, byte keysymsPerKeycode,
        byte keycodeCount,
        ReadOnlySpan<uint> keysyms) =>
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

    public ResponseProto XiAllowEvents(uint time, InputDevice deviceId, EventMode eventMode, uint touchId,
        uint grabWindow) =>
        XiAllowEventsBase(time, deviceId, eventMode, touchId, grabWindow);

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

    public ResponseProto SendExtensionEvent(uint destination, byte deviceId, bool propagate, ReadOnlySpan<int> classes,
        ReadOnlySpan<InputEvents> events) => SendExtensionEventBase(destination, deviceId, propagate, classes, events);
    
}