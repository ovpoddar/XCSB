using System;
using System.Numerics;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.Writers;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Infrastructure.VoidProto;

public interface IVoidProtoUnchecked
{
    void CloseDeviceUnChecked(byte deviceId);
    void SelectExtensionEventUnChecked(uint window, ReadOnlySpan<uint> classes); //xcb_input_event_class_t
    void ChangeDeviceDontPropagateListUnChecked(uint window, PropagateMode mode, ReadOnlySpan<uint> classes);
    void UngrabDeviceUnChecked(uint time, byte deviceId);
    void GrabDeviceKeyUnChecked(uint grabWindow, ushort modifiers, byte modifierDevice, byte grabbedDevice, byte key,
        GrabMode thisDeviceMode, GrabMode otherDeviceMode, bool ownerEvents, ReadOnlySpan<uint> classes); // modifier mask
    void UngrabDeviceKeyUnChecked(uint grabWindow, ushort modifiers, byte modifierDevice, byte key, byte grabbedDevice);// modifier mask
    void GrabDeviceButtonUnChecked(uint grabWindow, byte grabbedDevice, byte modifierDevice, ushort modifiers,
        GrabMode thisDeviceMode, GrabMode otherDeviceMode, byte button, bool ownerEvents, ReadOnlySpan<uint> classes);// modifier mask
    void UngrabDeviceButtonUnChecked(uint grabWindow, ushort modifiers, byte modifierDevice, byte button,
        byte grabbedDevice);// modifier mask
    void AllowDeviceEventsUnChecked(uint time, DeviceInputMode mode, byte deviceId);
    void SetDeviceFocusUnChecked(uint focus, uint time, InputFocusMode revertTo, byte deviceId);
    void ChangeFeedbackControlUnChecked<T>(FeedbackControlMask mask, byte deviceId, byte feedbackId, T feedback)
        where T : IFeedback;
    // suppose need changes
    void ChangeDeviceKeyMappingUnChecked(byte deviceId, byte firstKeycode, byte keysymsPerKeycode, byte keycodeCount,
        ReadOnlySpan<uint> keysyms);
    void DeviceBellUnChecked(byte deviceId, byte feedbackId, byte feedbackClass, sbyte percent);
    void ChangeDevicePropertyUnChecked<T>(ATOM property, ATOM type, byte deviceId, PropertyMode mode,
        ReadOnlySpan<T> items) where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    ;
    void DeleteDevicePropertyUnChecked(ATOM property, byte deviceId);
    void XiWarpPointerUnChecked(uint srcWin, uint dstWin, int srcX, int srcY, ushort srcWidth, ushort srcHeight,
        int dstX, int dstY, InputDevice deviceId);
    void XiChangeCursorUnChecked(uint window, uint cursor, InputDevice deviceId);
    void XiChangeHierarchyUnChecked(HierarchyChangeBuilder builder);
    void XiSetClientPointerUnChecked(uint window, InputDevice deviceId);
    void XiSelectEventsUnChecked(uint window, EventMaskBuilder mask);
    void XiSetFocusUnChecked(uint window, uint time, InputDevice deviceId);
    void XiUngrabDeviceUnChecked(uint time, InputDevice deviceId);
    void XiAllowEventsUnChecked(uint time, InputDevice deviceId, EventMode eventMode, uint touchId, uint grabWindow);
    void XiPassiveUngrabDeviceUnChecked(uint grabWindow, uint detail, InputDevice deviceId, GrabType grabType,
        ReadOnlySpan<uint> modifiers);
    void XiChangePropertyUnChecked<T>(InputDevice deviceId, PropertyMode mode, ATOM property, ATOM type,
        ReadOnlySpan<T> items) where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    ;
    void XiDeletePropertyUnChecked(InputDevice deviceId, ATOM property);
    void XiBarrierReleasePointerUnChecked(ReadOnlySpan<BarrierReleasePointerInfo> barriers);
    void SendExtensionEventUnChecked(uint destination, byte deviceId, bool propagate, ReadOnlySpan<int> classes,
        ReadOnlySpan<InputEvents> events);
}