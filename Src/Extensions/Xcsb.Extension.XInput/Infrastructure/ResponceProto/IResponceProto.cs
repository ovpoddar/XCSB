using Xcsb.Extension.XInput.Requests;
using Xcsb.Extension.XInput.Response.Replies;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Infrastructure.ResponceProto;

public interface IResponceProto
{
    ChangeDeviceControlReply ChangeDeviceControl(ushort controlId, byte deviceId);
    ListInputDevicesReply ListInputDevices();
    GetExtensionVersionReply GetExtensionVersion(ushort nameLen);
    OpenDeviceReply OpenDevice(byte deviceId);
    SetDeviceModeReply SetDeviceMode(byte deviceId, byte mode);
    GetSelectedExtensionEventsReply GetSelectedExtensionEvents(uint window);
    GetDeviceDontPropagateListReply GetDeviceDontPropagateList(uint window);
    GetDeviceMotionEventsReply GetDeviceMotionEvents(uint start, uint stop, byte deviceId);
    ChangeKeyboardDeviceReply ChangeKeyboardDevice(byte deviceId);
    ChangePointerDeviceReply ChangePointerDevice(byte xAxis, byte yAxis, byte deviceId);
    GrabDeviceReply GrabDevice(uint grabWindow, uint time, ushort numClasses, byte thisDeviceMode, byte otherDeviceMode,
        byte ownerEvents, byte deviceId);
    GetDeviceFocusReply GetDeviceFocus(byte deviceId);
    GetFeedbackControlReply GetFeedbackControl(byte deviceId);
    GetDeviceKeyMappingReply GetDeviceKeyMapping(byte deviceId, byte firstKeycode, byte count);
    GetDeviceModifierMappingReply GetDeviceModifierMapping(byte deviceId);
    SetDeviceModifierMappingReply SetDeviceModifierMapping(byte deviceId, byte keycodesPerModifier);
    GetDeviceButtonMappingReply GetDeviceButtonMapping(byte deviceId);
    SetDeviceButtonMappingReply SetDeviceButtonMapping(byte deviceId, byte mapSize);
    QueryDeviceStateReply QueryDeviceState(byte deviceId);
    SetDeviceValuatorsReply SetDeviceValuators(byte deviceId, byte firstValuator, byte numValuators);
    GetDeviceControlReply GetDeviceControl(ushort controlId, byte deviceId);
    ListDevicePropertiesReply ListDeviceProperties(byte deviceId);
    GetDevicePropertyReply GetDeviceProperty(ATOM property, ATOM type, uint offset, uint len, byte deviceId,
        byte delete);
    XiQueryPointerReply XiQueryPointer(uint window, ushort deviceid);
    XiGetClientPointerReply XiGetClientPointer(uint window);
    XiQueryVersionReply XiQueryVersion(ushort majorVersion, ushort minorVersion);
    XiQueryDeviceReply XiQueryDevice(ushort deviceid);
    XiGetFocusReply XiGetFocus(ushort deviceid);
    XiGrabDeviceReply XiGrabDevice(uint window, uint time, uint cursor, ushort deviceid, byte mode,
        byte pairedDeviceMode, byte ownerEvents, byte pad0, ushort maskLen);
    XiPassiveGrabDeviceReply XiPassiveGrabDevice(uint time, uint grabWindow, uint cursor, uint detail, ushort deviceid,
        ushort numModifiers, ushort maskLen, byte grabType, byte grabMode, byte pairedDeviceMode, byte ownerEvents);
    XiListPropertiesReply XiListProperties(ushort deviceid);
    XiGetPropertyReply XiGetProperty(ushort deviceid, byte delete, byte pad0, ATOM property, ATOM type, uint offset,
        uint len);
    XiGetSelectedEventsReply XiGetSelectedEvents(uint window);
}