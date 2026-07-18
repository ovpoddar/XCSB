using System;
using Xcsb.Connection;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Response.Replies;
using Xcsb.Extension.XInput.Infrastructure;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.TypeInfo;
using Xcsb.Extension.XInput.Requests;
using Xcsb.Extension.XInput.Response.Event;
using Xcsb.Extension.XInput.Response.Replies;
using Xcsb.Extension.XInput.Response.Replies.Internals;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Implementation;

// https://xorg.freedesktop.org/archive/X11R7.7/doc/libXi/inputlib.pdf
internal sealed partial class XInputProto : IXinputRequest
{
    private readonly QueryExtensionReply _response;
    private readonly IXExtensionInternal _extensionInternal;
    const int _minStackSupport = 512;

    public XInputProto(QueryExtensionReply response, IXExtensionInternal extensionInternal)
    {
        _response = response;
        _extensionInternal = extensionInternal;
        Resister(response.MajorOpcode, extensionInternal);
    }

    static void Resister(byte responseMajorOpcode, IXExtensionInternal extension)
    {
        extension.RegisterX1Event<DeviceValuatorEvent>(XiInputEventType.DeviceValuator);
        
        // extension.RegisterX2Event<DeviceChangedEvent>(responseMajorOpcode, XiInputEventType.DeviceChanged);
    }
    
    public GetExtensionVersionReply GetExtensionVersion(ReadOnlySpan<byte> name)
    {
        var cookie = GetExtensionVersionBase(name);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetExtensionVersionReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().ToStruct<GetExtensionVersionReply>();
    }

    public ChangeDeviceControlReply ChangeDeviceControl(DeviceControl controlId, byte deviceId)
    {
        var cookie = ChangeDeviceControlBase(controlId, deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<ChangeDeviceControlReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<ChangeDeviceControlReply>();
    }

    public ListInputDevicesReply ListInputDevices()
    {
        var cookie = ListInputDevicesBase();
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<ListInputDevicesResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new ListInputDevicesReply(result);
    }

    public OpenDeviceReply OpenDevice(byte deviceId)
    {
        var cookie = OpenDeviceBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<OpenDeviceReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<OpenDeviceReply>();
    }

    public SetDeviceModeReply SetDeviceMode(byte deviceId, ValuatorMode mode)
    {
        var cookie = SetDeviceModeBase(deviceId, mode);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<SetDeviceModeReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<SetDeviceModeReply>();
    }

    public GetSelectedExtensionEventsReply GetSelectedExtensionEvents(uint window)
    {
        var cookie = GetSelectedExtensionEventsBase(window);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetSelectedExtensionEventsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetSelectedExtensionEventsReply(result);
    }

    public GetDeviceDontPropagateListReply GetDeviceDontPropagateList(uint window)
    {
        var cookie = GetDeviceDontPropagateListBase(window);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDeviceDontPropagateListResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetDeviceDontPropagateListReply(result);
    }

    public GetDeviceMotionEventsReply GetDeviceMotionEvents(uint start, uint stop, byte deviceId)
    {
        var cookie = GetDeviceMotionEventsBase(start, stop, deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDeviceMotionEventsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetDeviceMotionEventsReply(result);
    }

    public ChangeKeyboardDeviceReply ChangeKeyboardDevice(byte deviceId)
    {
        var cookie = ChangeKeyboardDeviceBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<ChangeKeyboardDeviceReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<ChangeKeyboardDeviceReply>();
    }

    public ChangePointerDeviceReply ChangePointerDevice(byte xAxis, byte yAxis, byte deviceId)
    {
        var cookie = ChangePointerDeviceBase(xAxis, yAxis, deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<ChangePointerDeviceReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<ChangePointerDeviceReply>();
    }

    public GrabDeviceReply GrabDevice(uint grabWindow, uint time, GrabMode thisDeviceMode, GrabMode otherDeviceMode,
        bool ownerEvents, byte deviceId, ReadOnlySpan<uint> classes)
    {
        var cookie = GrabDeviceBase(grabWindow, time, thisDeviceMode, otherDeviceMode, ownerEvents, deviceId,
            classes);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GrabDeviceReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<GrabDeviceReply>();
    }

    public GetDeviceFocusReply GetDeviceFocus(byte deviceId)
    {
        var cookie = GetDeviceFocusBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDeviceFocusReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<GetDeviceFocusReply>();
    }

    public GetFeedbackControlReply GetFeedbackControl(byte deviceId)
    {
        var cookie = GetFeedbackControlBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetFeedbackControlResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetFeedbackControlReply(result);
    }

    public GetDeviceKeyMappingReply GetDeviceKeyMapping(byte deviceId, byte firstKeycode, byte count)
    {
        var cookie = GetDeviceKeyMappingBase(deviceId, firstKeycode, count);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDeviceKeyMappingResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetDeviceKeyMappingReply(result);
    }

    public GetDeviceModifierMappingReply GetDeviceModifierMapping(byte deviceId)
    {
        var cookie = GetDeviceModifierMappingBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDeviceModifierMappingResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetDeviceModifierMappingReply(result);
    }

    public SetDeviceModifierMappingReply SetDeviceModifierMapping(byte deviceId, ReadOnlySpan<uint> keycodesPerModifier)
    {
        var cookie = SetDeviceModifierMappingBase(deviceId, keycodesPerModifier);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<SetDeviceModifierMappingReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<SetDeviceModifierMappingReply>();
    }

    public GetDeviceButtonMappingReply GetDeviceButtonMapping(byte deviceId)
    {
        var cookie = GetDeviceButtonMappingBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDeviceButtonMappingReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<GetDeviceButtonMappingReply>();
    }

    public SetDeviceButtonMappingReply SetDeviceButtonMapping(byte deviceId, ReadOnlySpan<uint> map)
    {
        var cookie = SetDeviceButtonMappingBase(deviceId, map);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<SetDeviceButtonMappingReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<SetDeviceButtonMappingReply>();
    }

    public QueryDeviceStateReply QueryDeviceState(byte deviceId)
    {
        var cookie = QueryDeviceStateBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<QueryDeviceStateResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new QueryDeviceStateReply(result);
    }

    public SetDeviceValuatorsReply SetDeviceValuators(byte deviceId, byte firstValuator, ReadOnlySpan<uint> valuators)
    {
        var cookie = SetDeviceValuatorsBase(deviceId, firstValuator, valuators);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<SetDeviceValuatorsReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<SetDeviceValuatorsReply>();
    }

    public GetDeviceControlReply GetDeviceControl(DeviceControl controlId, byte deviceId)
    {
        var cookie = GetDeviceControlBase(controlId, deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDeviceControlReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<GetDeviceControlReply>();
    }

    public ListDevicePropertiesReply ListDeviceProperties(byte deviceId)
    {
        var cookie = ListDevicePropertiesBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<ListDevicePropertiesResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new ListDevicePropertiesReply(result);
    }

    public GetDevicePropertyReply GetDeviceProperty(ATOM property, ATOM type, uint offset, uint len, byte deviceId,
        bool delete)
    {
        var cookie = GetDevicePropertyBase(property, type, offset, len, deviceId, delete);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetDevicePropertyReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<GetDevicePropertyReply>();
    }

    public XiQueryPointerReply XiQueryPointer(uint window, InputDevice deviceId)
    {
        var cookie = XiQueryPointerBase(window, deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiQueryPointerResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new XiQueryPointerReply(result);
    }

    public XiGetClientPointerReply XiGetClientPointer(uint window)
    {
        var cookie = XiGetClientPointerBase(window);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiGetClientPointerReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<XiGetClientPointerReply>();
    }

    public XiQueryVersionReply XiQueryVersion(ushort majorVersion, ushort minorVersion)
    {
        var cookie = XiQueryVersionBase(majorVersion, minorVersion);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiQueryVersionReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<XiQueryVersionReply>();
    }

    public XiQueryDeviceReply XiQueryDevice(InputDevice deviceId)
    {
        var cookie = XiQueryDeviceBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiQueryDeviceResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new XiQueryDeviceReply(result);
    }

    public XiGetFocusReply XiGetFocus(InputDevice deviceId)
    {
        var cookie = XiGetFocusBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiGetFocusReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<XiGetFocusReply>();
    }

    public XiGrabDeviceReply XiGrabDevice(uint window, uint time, uint cursor, InputDevice deviceId, GrabMode mode,
        GrabMode pairedDeviceMode, GrabOwner ownerEvents, ReadOnlySpan<uint> classes)
    {
        var cookie = XiGrabDeviceBase(window, time, cursor, deviceId, mode, pairedDeviceMode, ownerEvents, classes);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiGrabDeviceReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<XiGrabDeviceReply>();
    }

    public XiPassiveGrabDeviceReply XiPassiveGrabDevice(uint time, uint grabWindow, uint cursor, uint detail,
        InputDevice deviceId, GrabType grabType, GrabMode22 grabMode, GrabMode pairedDeviceMode, GrabOwner ownerEvents,
        ReadOnlySpan<uint> mask, ReadOnlySpan<uint> modifiers)
    {
        var cookie = XiPassiveGrabDeviceBase(time, grabWindow, cursor, detail, deviceId, grabType, grabMode,
            pairedDeviceMode, ownerEvents, mask, modifiers);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiPassiveGrabDeviceResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new XiPassiveGrabDeviceReply(result);
    }

    public XiListPropertiesReply XiListProperties(InputDevice deviceId)
    {
        var cookie = XiListPropertiesBase(deviceId);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiListPropertiesResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new XiListPropertiesReply(result);
    }

    public XiGetPropertyReply XiGetProperty(InputDevice deviceId, byte delete, ATOM property, ATOM type, uint offset,
        uint len)
    {
        var cookie = XiGetPropertyBase(deviceId, delete, property, type, offset, len);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiGetPropertyReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().AsStruct<XiGetPropertyReply>();
    }

    public XiGetSelectedEventsReply XiGetSelectedEvents(uint window)
    {
        var cookie = XiGetSelectedEventsBase(window);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<XiGetSelectedEventsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new XiGetSelectedEventsReply(result);
    }
}