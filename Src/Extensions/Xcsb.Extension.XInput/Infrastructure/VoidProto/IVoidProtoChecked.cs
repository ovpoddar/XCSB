using System;
using System.Numerics;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.Writers;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Infrastructure.VoidProto;

public interface IVoidProtoChecked
{
   void CloseDeviceChecked(byte deviceId);
    void SelectExtensionEventChecked(uint window, ReadOnlySpan<uint> classes); //xcb_input_event_class_t
    void ChangeDeviceDontPropagateListChecked(uint window, PropagateMode mode, ReadOnlySpan<uint> classes);
    void UngrabDeviceChecked(uint time, byte deviceId);
    void GrabDeviceKeyChecked(uint grabWindow, ushort modifiers, byte modifierDevice, byte grabbedDevice, byte key,
        GrabMode thisDeviceMode, GrabMode otherDeviceMode, bool ownerEvents, ReadOnlySpan<uint> classes); // modifier mask
    void UngrabDeviceKeyChecked(uint grabWindow, ushort modifiers, byte modifierDevice, byte key, byte grabbedDevice);// modifier mask
    void GrabDeviceButtonChecked(uint grabWindow, byte grabbedDevice, byte modifierDevice, ushort modifiers,
        GrabMode thisDeviceMode, GrabMode otherDeviceMode, byte button, bool ownerEvents, ReadOnlySpan<uint> classes);// modifier mask
    void UngrabDeviceButtonChecked(uint grabWindow, ushort modifiers, byte modifierDevice, byte button,
        byte grabbedDevice);// modifier mask
    void AllowDeviceEventsChecked(uint time, DeviceInputMode mode, byte deviceId);
    void SetDeviceFocusChecked(uint focus, uint time, InputFocusMode revertTo, byte deviceId);
    void ChangeFeedbackControlChecked<T>(FeedbackControlMask mask, byte deviceId, byte feedbackId, T feedback)
        where T : IFeedback;
    // suppose need changes
    void ChangeDeviceKeyMappingChecked(byte deviceId, byte firstKeycode, byte keysymsPerKeycode, byte keycodeCount,
        ReadOnlySpan<uint> keysyms);
    void DeviceBellChecked(byte deviceId, byte feedbackId, byte feedbackClass, sbyte percent);
    void ChangeDevicePropertyChecked<T>(ATOM property, ATOM type, byte deviceId, PropertyMode mode,
        ReadOnlySpan<T> items) where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    ;
    void DeleteDevicePropertyChecked(ATOM property, byte deviceId);
    void XiWarpPointerChecked(uint srcWin, uint dstWin, int srcX, int srcY, ushort srcWidth, ushort srcHeight,
        int dstX, int dstY, InputDevice deviceId);
    void XiChangeCursorChecked(uint window, uint cursor, InputDevice deviceId);
    void XiChangeHierarchyChecked(HierarchyChangeBuilder builder);
    void XiSetClientPointerChecked(uint window, InputDevice deviceId);
    void XiSelectEventsChecked(uint window, EventMaskBuilder mask);
    void XiSetFocusChecked(uint window, uint time, InputDevice deviceId);
    void XiUngrabDeviceChecked(uint time, InputDevice deviceId);
    void XiAllowEventsChecked(uint time, InputDevice deviceId, EventMode eventMode, uint touchId, uint grabWindow);
    void XiPassiveUngrabDeviceChecked(uint grabWindow, uint detail, InputDevice deviceId, GrabType grabType,
        ReadOnlySpan<uint> modifiers);
    void XiChangePropertyChecked<T>(InputDevice deviceId, PropertyMode mode, ATOM property, ATOM type,
        ReadOnlySpan<T> items) where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    ;
    void XiDeletePropertyChecked(InputDevice deviceId, ATOM property);
    void XiBarrierReleasePointerChecked(ReadOnlySpan<BarrierReleasePointerInfo> barriers);
    void SendExtensionEventChecked(uint destination, byte deviceId, bool propagate, ReadOnlySpan<int> classes,
        ReadOnlySpan<InputEvents> events);
}