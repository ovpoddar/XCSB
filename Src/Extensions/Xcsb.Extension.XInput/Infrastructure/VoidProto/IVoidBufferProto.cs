using System;
using System.Numerics;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.Writers;
using Xcsb.Masks;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Infrastructure.VoidProto;

public interface IVoidBufferProto
{
    void CloseDevice(byte deviceId);
    void SelectExtensionEvent(uint window, ReadOnlySpan<uint> classes); //xcb_input_event_class_t
    void ChangeDeviceDontPropagateList(uint window, PropagateMode mode, ReadOnlySpan<uint> classes);
    void UngrabDevice(uint time, byte deviceId);
    void GrabDeviceKey(uint grabWindow, ModifierMask modifiers, byte modifierDevice, byte grabbedDevice,
        byte key, GrabMode thisDeviceMode, GrabMode otherDeviceMode, bool ownerEvents, ReadOnlySpan<uint> classes);
    void UngrabDeviceKey(uint grabWindow, ModifierMask modifiers, byte modifierDevice, byte key,
        byte grabbedDevice);
    void GrabDeviceButton(uint grabWindow, byte grabbedDevice, byte modifierDevice, ModifierMask modifiers,
        GrabMode thisDeviceMode, GrabMode otherDeviceMode, byte button, bool ownerEvents, ReadOnlySpan<uint> classes);
    void UngrabDeviceButton(uint grabWindow, ModifierMask modifiers, byte modifierDevice, byte button,
        byte grabbedDevice);
    void AllowDeviceEvents(uint time, DeviceInputMode mode, byte deviceId);
    void SetDeviceFocus(uint focus, uint time, InputFocusMode revertTo, byte deviceId);
    void ChangeFeedbackControl<T>(FeedbackControlMask mask, byte deviceId, byte feedbackId, T feedback)
        where T : IFeedback;

    // suppose need changes
    void ChangeDeviceKeyMapping(byte deviceId, byte firstKeycode, byte keysymsPerKeycode, byte keycodeCount,
        ReadOnlySpan<uint> keysyms);
    void DeviceBell(byte deviceId, byte feedbackId, byte feedbackClass, sbyte percent);
    void ChangeDeviceProperty<T>(ATOM property, ATOM type, byte deviceId, PropertyMode mode,
        ReadOnlySpan<T> items) where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    ;
    void DeleteDeviceProperty(ATOM property, byte deviceId);
    void XiWarpPointer(uint srcWin, uint dstWin, int srcX, int srcY, ushort srcWidth, ushort srcHeight,
        int dstX, int dstY, InputDevice deviceId);
    void XiChangeCursor(uint window, uint cursor, InputDevice deviceId);
    void XiChangeHierarchy(HierarchyChangeBuilder builder);
    void XiSetClientPointer(uint window, InputDevice deviceId);
    void XiSelectEvents(uint window, EventMaskBuilder mask);
    void XiSetFocus(uint window, uint time, InputDevice deviceId);
    void XiUngrabDevice(uint time, InputDevice deviceId);
    void XiAllowEvents(uint time, InputDevice deviceId, EventMode eventMode, uint touchId, uint grabWindow);
    void XiPassiveUngrabDevice(uint grabWindow, uint detail, InputDevice deviceId, GrabType grabType,
        ReadOnlySpan<uint> modifiers);
    void XiChangeProperty<T>(InputDevice deviceId, PropertyMode mode, ATOM property, ATOM type,
        ReadOnlySpan<T> items) where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    ;
    void XiDeleteProperty(InputDevice deviceId, ATOM property);
    void XiBarrierReleasePointer(ReadOnlySpan<BarrierReleasePointerInfo> barriers);
    void SendExtensionEvent(uint destination, byte deviceId, bool propagate, ReadOnlySpan<int> classes,
        ReadOnlySpan<InputEvents> events);
}