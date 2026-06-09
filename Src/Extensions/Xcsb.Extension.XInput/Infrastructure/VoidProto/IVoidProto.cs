using System;
using Xcsb.Connection.Response;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.Writers;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Infrastructure.VoidProto;

public interface IVoidProto
{
    ResponseProto CloseDevice(byte deviceId);
    ResponseProto SelectExtensionEvent(uint window, ushort numClasses, uint[] foo);
    ResponseProto ChangeDeviceDontPropagateList(uint window, ushort numClasses, byte mode, uint[] foo);
    ResponseProto UngrabDevice(uint time, byte deviceId);
    ResponseProto GrabDeviceKey(uint grabWindow, ushort numClasses, ushort modifiers, byte modifierDevice,
        byte grabbedDevice, byte key, byte thisDeviceMode, byte otherDeviceMode, byte ownerEvents, uint[] foo);
    ResponseProto UngrabDeviceKey(uint grabWindow, ushort modifiers, byte modifierDevice, byte key, byte grabbedDevice);
    ResponseProto GrabDeviceButton(uint grabWindow, byte grabbedDevice, byte modifierDevice, ushort numClasses,
        ushort modifiers, byte thisDeviceMode, byte otherDeviceMode, byte button, byte ownerEvents, uint[] foo);
    ResponseProto UngrabDeviceButton(uint grabWindow, ushort modifiers, byte modifierDevice, byte button,
        byte grabbedDevice);
    ResponseProto AllowDeviceEvents(uint time, byte mode, byte deviceId);
    ResponseProto SetDeviceFocus(uint focus, uint time, byte revertTo, byte deviceId);
    ResponseProto ChangeFeedbackControl<T>(FeedbackControlMask mask, byte deviceId, byte feedbackId, T feedback) 
        where T : IFeedback;
    ResponseProto ChangeDeviceKeyMapping(byte deviceId, byte firstKeycode, byte keysymsPerKeycode, byte keycodeCount,
        uint[] foo);
    ResponseProto DeviceBell(byte deviceId, byte feedbackId, byte feedbackClass, sbyte percent);
    ResponseProto ChangeDeviceProperty(ATOM property, ATOM type, byte deviceId, byte format, byte mode, uint numItems,
        byte[] foo);
    ResponseProto DeleteDeviceProperty(ATOM property, byte deviceId);
    ResponseProto XiWarpPointer(uint srcWin, uint dstWin, int srcX, int srcY, ushort srcWidth, ushort srcHeight,
        int dstX, int dstY, ushort deviceId);
    ResponseProto XiChangeCursor(uint window, uint cursor, ushort deviceId);
    ResponseProto XiChangeHierarchy(HierarchyChangeBuilder builder);
    ResponseProto XiSetClientPointer(uint window, ushort deviceId);
    ResponseProto XiSelectEvents(uint window, EventMaskBuilder mask);
    ResponseProto XiSetFocus(uint window, uint time, ushort deviceId);
    ResponseProto XiUngrabDevice(uint time, ushort deviceId);
    ResponseProto XiAllowEvents(uint time, ushort deviceId, byte eventMode, uint touchId, uint grabWindow);
    ResponseProto XiPassiveUngrabDevice(uint grabWindow, uint detail, ushort deviceId, ushort numModifiers,
        byte grabType, uint[] foo);
    ResponseProto XiChangeProperty(ushort deviceId, byte mode, byte format, ATOM property, ATOM type, uint numItems,
        byte[] foo);
    ResponseProto XiDeleteProperty(ushort deviceId, ATOM property);
    ResponseProto XiBarrierReleasePointer(ReadOnlySpan<BarrierReleasePointerInfo> barriers);
    ResponseProto SendExtensionEvent(uint destination, byte deviceId, byte propagate, ushort numClasses, byte numEvents,
        int[] foo);
}