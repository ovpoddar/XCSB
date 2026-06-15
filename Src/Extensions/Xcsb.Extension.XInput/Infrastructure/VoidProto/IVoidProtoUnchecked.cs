using System;
using System.Numerics;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.Writers;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Infrastructure.VoidProto;

public interface IVoidProtoUnchecked
{
    void CloseDeviceUnchecked(byte deviceId);
    void SelectExtensionEventUnchecked(uint window, ReadOnlySpan<uint> classes); //xcb_input_event_class_t
    void ChangeDeviceDontPropagateListUnchecked(uint window, byte mode, ReadOnlySpan<uint> classes);
    void UngrabDeviceUnchecked(uint time, byte deviceId);

    void GrabDeviceKeyUnchecked(uint grabWindow, ushort modifiers, byte modifierDevice, byte grabbedDevice, byte key,
        byte thisDeviceMode, byte otherDeviceMode, byte ownerEvents, ReadOnlySpan<uint> classes);

    void UngrabDeviceKeyUnchecked(uint grabWindow, ushort modifiers, byte modifierDevice, byte key, byte grabbedDevice);

    void GrabDeviceButtonUnchecked(uint grabWindow, byte grabbedDevice, byte modifierDevice, ushort modifiers,
        byte thisDeviceMode, byte otherDeviceMode, byte button, byte ownerEvents, ReadOnlySpan<uint> classes);

    void UngrabDeviceButtonUnchecked(uint grabWindow, ushort modifiers, byte modifierDevice, byte button,
        byte grabbedDevice);

    void AllowDeviceEventsUnchecked(uint time, byte mode, byte deviceId);
    void SetDeviceFocusUnchecked(uint focus, uint time, byte revertTo, byte deviceId);

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
        int dstX, int dstY, ushort deviceId);

    void XiChangeCursorUnchecked(uint window, uint cursor, ushort deviceId);
    void XiChangeHierarchyUnchecked(HierarchyChangeBuilder builder);
    void XiSetClientPointerUnchecked(uint window, ushort deviceId);
    void XiSelectEventsUnchecked(uint window, EventMaskBuilder mask);
    void XiSetFocusUnchecked(uint window, uint time, ushort deviceId);
    void XiUngrabDeviceUnchecked(uint time, ushort deviceId);
    void XiAllowEventsUnchecked(uint time, ushort deviceId, byte eventMode, uint touchId, uint grabWindow);

    void XiPassiveUngrabDeviceUnchecked(uint grabWindow, uint detail, InputDevice deviceId, GrabType grabType,
        ReadOnlySpan<uint> modifiers);

    void XiChangePropertyUnchecked<T>(ushort deviceId, PropertyMode mode, ATOM property, ATOM type,
        ReadOnlySpan<T> items) where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    ;

    void XiDeletePropertyUnchecked(ushort deviceId, ATOM property);
    void XiBarrierReleasePointerUnchecked(ReadOnlySpan<BarrierReleasePointerInfo> barriers);

    void SendExtensionEventUnchecked(uint destination, byte deviceId, byte propagate, byte numEvents,
        ReadOnlySpan<int> foo);
}