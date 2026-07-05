using System;
using System.Numerics;
using Xcsb.Connection.Response;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.Writers;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Infrastructure.VoidProto;

public interface IVoidProto
{
    ResponseProto CloseDevice(byte deviceId);
    ResponseProto SelectExtensionEvent(uint window, ReadOnlySpan<uint> classes); //xcb_input_event_class_t
    ResponseProto ChangeDeviceDontPropagateList(uint window, PropagateMode mode, ReadOnlySpan<uint> classes);
    ResponseProto UngrabDevice(uint time, byte deviceId);
    ResponseProto GrabDeviceKey(uint grabWindow, ushort modifiers, byte modifierDevice, byte grabbedDevice, byte key,
        GrabMode thisDeviceMode, GrabMode otherDeviceMode, bool ownerEvents, ReadOnlySpan<uint> classes); // modifier mask
    ResponseProto UngrabDeviceKey(uint grabWindow, ushort modifiers, byte modifierDevice, byte key, byte grabbedDevice);// modifier mask
    ResponseProto GrabDeviceButton(uint grabWindow, byte grabbedDevice, byte modifierDevice, ushort modifiers,
        GrabMode thisDeviceMode, GrabMode otherDeviceMode, byte button, bool ownerEvents, ReadOnlySpan<uint> classes);// modifier mask
    ResponseProto UngrabDeviceButton(uint grabWindow, ushort modifiers, byte modifierDevice, byte button,
        byte grabbedDevice);// modifier mask
    ResponseProto AllowDeviceEvents(uint time, DeviceInputMode mode, byte deviceId);
    ResponseProto SetDeviceFocus(uint focus, uint time, InputFocusMode revertTo, byte deviceId);
    ResponseProto ChangeFeedbackControl<T>(FeedbackControlMask mask, byte deviceId, byte feedbackId, T feedback)
        where T : IFeedback;
    // suppose need changes
    ResponseProto ChangeDeviceKeyMapping(byte deviceId, byte firstKeycode, byte keysymsPerKeycode, byte keycodeCount,
        ReadOnlySpan<uint> keysyms);
    ResponseProto DeviceBell(byte deviceId, byte feedbackId, byte feedbackClass, sbyte percent);
    ResponseProto ChangeDeviceProperty<T>(ATOM property, ATOM type, byte deviceId, PropertyMode mode,
        ReadOnlySpan<T> items) where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    ;
    ResponseProto DeleteDeviceProperty(ATOM property, byte deviceId);
    ResponseProto XiWarpPointer(uint srcWin, uint dstWin, int srcX, int srcY, ushort srcWidth, ushort srcHeight,
        int dstX, int dstY, InputDevice deviceId);
    ResponseProto XiChangeCursor(uint window, uint cursor, InputDevice deviceId);
    ResponseProto XiChangeHierarchy(HierarchyChangeBuilder builder);
    ResponseProto XiSetClientPointer(uint window, InputDevice deviceId);
    ResponseProto XiSelectEvents(uint window, EventMaskBuilder mask);
    ResponseProto XiSetFocus(uint window, uint time, InputDevice deviceId);
    ResponseProto XiUngrabDevice(uint time, InputDevice deviceId);
    ResponseProto XiAllowEvents(uint time, InputDevice deviceId, EventMode eventMode, uint touchId, uint grabWindow);
    ResponseProto XiPassiveUngrabDevice(uint grabWindow, uint detail, InputDevice deviceId, GrabType grabType,
        ReadOnlySpan<uint> modifiers);
    ResponseProto XiChangeProperty<T>(InputDevice deviceId, PropertyMode mode, ATOM property, ATOM type,
        ReadOnlySpan<T> items) where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    ;
    ResponseProto XiDeleteProperty(InputDevice deviceId, ATOM property);
    ResponseProto XiBarrierReleasePointer(ReadOnlySpan<BarrierReleasePointerInfo> barriers);
    ResponseProto SendExtensionEvent(uint destination, byte deviceId, bool propagate, ReadOnlySpan<int> classes,
        ReadOnlySpan<InputEvents> events);
}