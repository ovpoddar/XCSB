using System;
using System.Numerics;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.Writers;
using Xcsb.Masks;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Infrastructure.VoidProto;

public interface IVoidProtoUnchecked
{
    void CloseDeviceUnchecked(byte deviceId);
    void SelectExtensionEventUnchecked(uint window, ReadOnlySpan<uint> classes); //xcb_input_event_class_t
    void ChangeDeviceDontPropagateListUnchecked(uint window, PropagateMode mode, ReadOnlySpan<uint> classes);
    void UngrabDeviceUnchecked(uint time, byte deviceId);
    void GrabDeviceKeyUnchecked(uint grabWindow, ModifierMask modifiers, byte modifierDevice, byte grabbedDevice,
        byte key, GrabMode thisDeviceMode, GrabMode otherDeviceMode, bool ownerEvents, ReadOnlySpan<uint> classes);
    void UngrabDeviceKeyUnchecked(uint grabWindow, ModifierMask modifiers, byte modifierDevice, byte key,
        byte grabbedDevice);
    void GrabDeviceButtonUnchecked(uint grabWindow, byte grabbedDevice, byte modifierDevice, ModifierMask modifiers,
        GrabMode thisDeviceMode, GrabMode otherDeviceMode, byte button, bool ownerEvents, ReadOnlySpan<uint> classes);
    void UngrabDeviceButtonUnchecked(uint grabWindow, ModifierMask modifiers, byte modifierDevice, byte button,
        byte grabbedDevice);
    void AllowDeviceEventsUnchecked(uint time, DeviceInputMode mode, byte deviceId);
    void SetDeviceFocusUnchecked(uint focus, uint time, InputFocusMode revertTo, byte deviceId);
    void ChangeFeedbackControlUnchecked<T>(FeedbackControlMask mask, byte deviceId, byte feedbackId, T feedback)
        where T : IFeedback;

    // suppose need changes
    void ChangeDeviceKeyMappingUnchecked(byte deviceId, byte firstKeycode, byte keysymsPerKeycode, byte keycodeCount,
        ReadOnlySpan<uint> keysyms);
    void DeviceBellUnchecked(byte deviceId, byte feedbackId, byte feedbackClass, sbyte percent);
    void ChangeDevicePropertyUnchecked<T>(ATOM property, ATOM type, byte deviceId, PropertyMode mode,
        ReadOnlySpan<T> items) where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    ;
    void DeleteDevicePropertyUnchecked(ATOM property, byte deviceId);
    void XiWarpPointerUnchecked(uint srcWin, uint dstWin, int srcX, int srcY, ushort srcWidth, ushort srcHeight,
        int dstX, int dstY, InputDevice deviceId);
    void XiChangeCursorUnchecked(uint window, uint cursor, InputDevice deviceId);
    void XiChangeHierarchyUnchecked(HierarchyChangeBuilder builder);
    void XiSetClientPointerUnchecked(uint window, InputDevice deviceId);
    void XiSelectEventsUnchecked(uint window, EventMaskBuilder mask);
    void XiSetFocusUnchecked(uint window, uint time, InputDevice deviceId);
    void XiUngrabDeviceUnchecked(uint time, InputDevice deviceId);
    void XiAllowEventsUnchecked(uint time, InputDevice deviceId, EventMode eventMode, uint touchId, uint grabWindow);
    void XiPassiveUngrabDeviceUnchecked(uint grabWindow, uint detail, InputDevice deviceId, GrabType grabType,
        ReadOnlySpan<uint> modifiers);
    void XiChangePropertyUnchecked<T>(InputDevice deviceId, PropertyMode mode, ATOM property, ATOM type,
        ReadOnlySpan<T> items) where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    ;
    void XiDeletePropertyUnchecked(InputDevice deviceId, ATOM property);
    void XiBarrierReleasePointerUnchecked(ReadOnlySpan<BarrierReleasePointerInfo> barriers);
    void SendExtensionEventUnchecked(uint destination, byte deviceId, bool propagate, ReadOnlySpan<int> classes,
        ReadOnlySpan<InputEvents> events);
}