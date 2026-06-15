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
    void ChangeDeviceDontPropagateListChecked(uint window, byte mode, ReadOnlySpan<uint> classes);
    void UngrabDeviceChecked(uint time, byte deviceId);

    void GrabDeviceKeyChecked(uint grabWindow, ushort modifiers, byte modifierDevice, byte grabbedDevice, byte key,
        byte thisDeviceMode, byte otherDeviceMode, byte ownerEvents, ReadOnlySpan<uint> classes);

    void UngrabDeviceKeyChecked(uint grabWindow, ushort modifiers, byte modifierDevice, byte key, byte grabbedDevice);

    void GrabDeviceButtonChecked(uint grabWindow, byte grabbedDevice, byte modifierDevice, ushort modifiers,
        byte thisDeviceMode, byte otherDeviceMode, byte button, byte ownerEvents, ReadOnlySpan<uint> classes);

    void UngrabDeviceButtonChecked(uint grabWindow, ushort modifiers, byte modifierDevice, byte button,
        byte grabbedDevice);

    void AllowDeviceEventsChecked(uint time, byte mode, byte deviceId);
    void SetDeviceFocusChecked(uint focus, uint time, byte revertTo, byte deviceId);

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
        int dstX, int dstY, ushort deviceId);

    void XiChangeCursorChecked(uint window, uint cursor, ushort deviceId);
    void XiChangeHierarchyChecked(HierarchyChangeBuilder builder);
    void XiSetClientPointerChecked(uint window, ushort deviceId);
    void XiSelectEventsChecked(uint window, EventMaskBuilder mask);
    void XiSetFocusChecked(uint window, uint time, ushort deviceId);
    void XiUngrabDeviceChecked(uint time, ushort deviceId);
    void XiAllowEventsChecked(uint time, ushort deviceId, byte eventMode, uint touchId, uint grabWindow);

    void XiPassiveUngrabDeviceChecked(uint grabWindow, uint detail, InputDevice deviceId, GrabType grabType,
        ReadOnlySpan<uint> modifiers);

    void XiChangePropertyChecked<T>(ushort deviceId, PropertyMode mode, ATOM property, ATOM type,
        ReadOnlySpan<T> items) where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    ;

    void XiDeletePropertyChecked(ushort deviceId, ATOM property);
    void XiBarrierReleasePointerChecked(ReadOnlySpan<BarrierReleasePointerInfo> barriers);

    void SendExtensionEventChecked(uint destination, byte deviceId, byte propagate, byte numEvents,
        ReadOnlySpan<int> foo);
}