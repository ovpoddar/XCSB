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

    public void ChangeDeviceDontPropagateListUnchecked(uint window, PropagateMode mode, ReadOnlySpan<uint> classes)
    {
        var cookie = ChangeDeviceDontPropagateListBase(window, mode, classes);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void UngrabDeviceUnchecked(uint time, byte deviceId)
    {
        var cookie = UngrabDeviceBase(time, deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void GrabDeviceKeyUnchecked(uint grabWindow, ModifierMask modifiers, byte modifierDevice, byte grabbedDevice,
        byte key, GrabMode thisDeviceMode, GrabMode otherDeviceMode, bool ownerEvents, ReadOnlySpan<uint> classes)
    {
        var cookie = GrabDeviceKeyBase(grabWindow, modifiers, modifierDevice, grabbedDevice, key, thisDeviceMode,
                otherDeviceMode, ownerEvents, classes);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }


    public void UngrabDeviceKeyUnchecked(uint grabWindow, ModifierMask modifiers, byte modifierDevice, byte key,
        byte grabbedDevice)
    {
        var cookie = UngrabDeviceKeyBase(grabWindow, modifiers, modifierDevice, key, grabbedDevice);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void GrabDeviceButtonUnchecked(uint grabWindow, byte grabbedDevice, byte modifierDevice,
        ModifierMask modifiers, GrabMode thisDeviceMode, GrabMode otherDeviceMode, byte button, bool ownerEvents, 
        ReadOnlySpan<uint> classes)
    {
        var cookie = GrabDeviceButtonBase(grabWindow, grabbedDevice, modifierDevice, modifiers,
            thisDeviceMode, otherDeviceMode, button, ownerEvents, classes);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void UngrabDeviceButtonUnchecked(uint grabWindow, ModifierMask modifiers, byte modifierDevice, byte button,
        byte grabbedDevice)
    {
        var cookie = UngrabDeviceButtonBase(grabWindow, modifiers, modifierDevice, button, grabbedDevice);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void AllowDeviceEventsUnchecked(uint time, DeviceInputMode mode, byte deviceId)
    {
        var cookie = AllowDeviceEventsBase(time, mode, deviceId);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }

    public void SetDeviceFocusUnchecked(uint focus, uint time, InputFocusMode revertTo, byte deviceId)
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
        byte keycodeCount,
        ReadOnlySpan<uint> keysyms)
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

    public void XiAllowEventsUnchecked(uint time, InputDevice deviceId, EventMode eventMode, uint touchId,
        uint grabWindow)
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

    public void SendExtensionEventUnchecked(uint destination, byte deviceId, bool propagate, ReadOnlySpan<int> classes,
        ReadOnlySpan<InputEvents> events)
    {
        var cookie = SendExtensionEventBase(destination, deviceId, propagate, classes, events);
        _extensionInternal.Transport.SkipErrorForSequence(cookie.Id, false);
    }
}