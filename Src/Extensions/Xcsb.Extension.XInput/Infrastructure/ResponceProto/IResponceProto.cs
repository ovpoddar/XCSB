using System;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Requests;
using Xcsb.Extension.XInput.Response.Replies;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Infrastructure.ResponceProto;

public interface IResponceProto
{
    ChangeDeviceControlReply ChangeDeviceControl(DeviceControl controlId, byte deviceId);
    ListInputDevicesReply ListInputDevices();
    OpenDeviceReply OpenDevice(byte deviceId);
    SetDeviceModeReply SetDeviceMode(byte deviceId, ValuatorMode mode);
    GetSelectedExtensionEventsReply GetSelectedExtensionEvents(uint window);
    GetDeviceDontPropagateListReply GetDeviceDontPropagateList(uint window);
    GetDeviceMotionEventsReply GetDeviceMotionEvents(uint start, uint stop, byte deviceId);
    ChangeKeyboardDeviceReply ChangeKeyboardDevice(byte deviceId);
    ChangePointerDeviceReply ChangePointerDevice(byte xAxis, byte yAxis, byte deviceId);
    GrabDeviceReply GrabDevice(uint grabWindow, uint time, GrabMode thisDeviceMode, GrabMode otherDeviceMode,
        bool ownerEvents, byte deviceId, ReadOnlySpan<uint> classes);
    GetDeviceFocusReply GetDeviceFocus(byte deviceId);
    GetFeedbackControlReply GetFeedbackControl(byte deviceId);
    GetDeviceKeyMappingReply GetDeviceKeyMapping(byte deviceId, byte firstKeycode, byte count);
    GetDeviceModifierMappingReply GetDeviceModifierMapping(byte deviceId);
    SetDeviceModifierMappingReply SetDeviceModifierMapping(byte deviceId, ReadOnlySpan<uint> keycodesPerModifier);
    GetDeviceButtonMappingReply GetDeviceButtonMapping(byte deviceId);
    SetDeviceButtonMappingReply SetDeviceButtonMapping(byte deviceId, ReadOnlySpan<uint> map);
    QueryDeviceStateReply QueryDeviceState(byte deviceId);
    SetDeviceValuatorsReply SetDeviceValuators(byte deviceId, byte firstValuator, ReadOnlySpan<uint>valuators);
    GetDeviceControlReply GetDeviceControl(DeviceControl controlId, byte deviceId);
    ListDevicePropertiesReply ListDeviceProperties(byte deviceId);
    GetDevicePropertyReply GetDeviceProperty(ATOM property, ATOM type, uint offset, uint len, byte deviceId,
        bool delete);
    XiQueryPointerReply XiQueryPointer(uint window, InputDevice deviceId);
    XiGetClientPointerReply XiGetClientPointer(uint window);
    XiQueryVersionReply XiQueryVersion(ushort majorVersion, ushort minorVersion);
    XiQueryDeviceReply XiQueryDevice(InputDevice deviceId);
    XiGetFocusReply XiGetFocus(InputDevice deviceId);
    XiGrabDeviceReply XiGrabDevice(uint window, uint time, uint cursor, InputDevice deviceId, GrabMode mode,
        GrabMode pairedDeviceMode, GrabOwner ownerEvents, ReadOnlySpan<uint> classes);
    XiPassiveGrabDeviceReply XiPassiveGrabDevice(uint time, uint grabWindow, uint cursor, uint detail,
        InputDevice deviceId, GrabType grabType, GrabMode22 grabMode, GrabMode pairedDeviceMode, GrabOwner ownerEvents, 
        ReadOnlySpan<uint> mask, ReadOnlySpan<uint> modifiers);
    XiListPropertiesReply XiListProperties(InputDevice deviceId);
    XiGetPropertyReply XiGetProperty(InputDevice deviceId, byte delete, ATOM property, ATOM type, uint offset,
        uint len);
    XiGetSelectedEventsReply XiGetSelectedEvents(uint window);
}