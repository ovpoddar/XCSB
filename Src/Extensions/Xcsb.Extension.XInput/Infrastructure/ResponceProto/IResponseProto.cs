using System;
using Xcsb.Connection.Models;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Requests;
using Xcsb.Extension.XInput.Response.Replies;
using Xcsb.Generators;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Infrastructure.ResponceProto;

[Declaration(DeclarationKind.Async)] 
public interface IResponseProto
{
    ReplyLease<ChangeDeviceControlReply> ChangeDeviceControl(DeviceControl controlId, byte deviceId);
    ReplyLease<ListInputDevicesReply> ListInputDevices();
    ReplyLease<OpenDeviceReply> OpenDevice(byte deviceId);
    ReplyLease<SetDeviceModeReply> SetDeviceMode(byte deviceId, ValuatorMode mode);
    ReplyLease<GetSelectedExtensionEventsReply> GetSelectedExtensionEvents(uint window);
    ReplyLease<GetDeviceDontPropagateListReply> GetDeviceDontPropagateList(uint window);
    ReplyLease<GetDeviceMotionEventsReply> GetDeviceMotionEvents(uint start, uint stop, byte deviceId);
    ReplyLease<ChangeKeyboardDeviceReply> ChangeKeyboardDevice(byte deviceId);
    ReplyLease<ChangePointerDeviceReply> ChangePointerDevice(byte xAxis, byte yAxis, byte deviceId);
    ReplyLease<GrabDeviceReply> GrabDevice(uint grabWindow, uint time, GrabMode thisDeviceMode, GrabMode otherDeviceMode,
        bool ownerEvents, byte deviceId, ReadOnlySpan<uint> classes);
    ReplyLease<GetDeviceFocusReply> GetDeviceFocus(byte deviceId);
    ReplyLease<GetFeedbackControlReply> GetFeedbackControl(byte deviceId);
    ReplyLease<GetDeviceKeyMappingReply> GetDeviceKeyMapping(byte deviceId, byte firstKeycode, byte count);
    ReplyLease<GetDeviceModifierMappingReply> GetDeviceModifierMapping(byte deviceId);
    ReplyLease<SetDeviceModifierMappingReply> SetDeviceModifierMapping(byte deviceId, ReadOnlySpan<uint> keycodesPerModifier);
    ReplyLease<GetDeviceButtonMappingReply> GetDeviceButtonMapping(byte deviceId);
    ReplyLease<SetDeviceButtonMappingReply> SetDeviceButtonMapping(byte deviceId, ReadOnlySpan<uint> map);
    ReplyLease<QueryDeviceStateReply> QueryDeviceState(byte deviceId);
    ReplyLease<SetDeviceValuatorsReply> SetDeviceValuators(byte deviceId, byte firstValuator, ReadOnlySpan<uint>valuators);
    ReplyLease<GetDeviceControlReply> GetDeviceControl(DeviceControl controlId, byte deviceId);
    ReplyLease<ListDevicePropertiesReply> ListDeviceProperties(byte deviceId);
    ReplyLease<GetDevicePropertyReply> GetDeviceProperty(ATOM property, ATOM type, uint offset, uint len, byte deviceId,
        bool delete);
    ReplyLease<XiQueryPointerReply> XiQueryPointer(uint window, InputDevice deviceId);
    ReplyLease<XiGetClientPointerReply> XiGetClientPointer(uint window);
    ReplyLease<XiQueryVersionReply> XiQueryVersion(ushort majorVersion, ushort minorVersion);
    ReplyLease<XiQueryDeviceReply> XiQueryDevice(InputDevice deviceId);
    ReplyLease<XiGetFocusReply> XiGetFocus(InputDevice deviceId);
    ReplyLease<XiGrabDeviceReply> XiGrabDevice(uint window, uint time, uint cursor, InputDevice deviceId, GrabMode mode,
        GrabMode pairedDeviceMode, GrabOwner ownerEvents, ReadOnlySpan<uint> classes);
    ReplyLease<XiPassiveGrabDeviceReply> XiPassiveGrabDevice(uint time, uint grabWindow, uint cursor, uint detail,
        InputDevice deviceId, GrabType grabType, GrabMode22 grabMode, GrabMode pairedDeviceMode, GrabOwner ownerEvents, 
        ReadOnlySpan<uint> mask, ReadOnlySpan<uint> modifiers);
    ReplyLease<XiListPropertiesReply> XiListProperties(InputDevice deviceId);
    ReplyLease<XiGetPropertyReply> XiGetProperty(InputDevice deviceId, byte delete, ATOM property, ATOM type, uint offset,
        uint len);
    ReplyLease<XiGetSelectedEventsReply> XiGetSelectedEvents(uint window);
}