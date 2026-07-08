using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Requests;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Implementation;

internal sealed partial class XInputProto
{
    private ResponseProto GetExtensionVersionBase(ReadOnlySpan<byte> name)
    {
        var request = new GetExtensionVersionType(this._response.MajorOpcode, (ushort)name.Length);
        var requestSize = request.Length * 4;
        if (requestSize < _minStackSupport)
        {
            Span<byte> scratchBuffer = stackalloc byte[requestSize];
            scratchBuffer.WriteRequest(ref request, 8, name);
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requestSize);
            scratchBuffer[..requestSize].WriteRequest(ref request, 8, name);
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer[..requestSize], SocketFlags.None);
        }

        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto ChangeDeviceControlBase(DeviceControl controlId, byte deviceId)
    {
        var request = new ChangeDeviceControlType(this._response.MajorOpcode, controlId, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto ListInputDevicesBase()
    {
        var request = new ListInputDevicesType(this._response.MajorOpcode);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto OpenDeviceBase(byte deviceId)
    {
        var request = new OpenDeviceType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto SetDeviceModeBase(byte deviceId, ValuatorMode mode)
    {
        var request = new SetDeviceModeType(this._response.MajorOpcode, deviceId, mode);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GetSelectedExtensionEventsBase(uint window)
    {
        var request = new GetSelectedExtensionEventsType(this._response.MajorOpcode, window);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GetDeviceDontPropagateListBase(uint window)
    {
        var request = new GetDeviceDontPropagateListType(this._response.MajorOpcode, window);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GetDeviceMotionEventsBase(uint start, uint stop, byte deviceId)
    {
        var request = new GetDeviceMotionEventsType(this._response.MajorOpcode, start, stop, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto ChangeKeyboardDeviceBase(byte deviceId)
    {
        var request = new ChangeKeyboardDeviceType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto ChangePointerDeviceBase(byte xAxis, byte yAxis, byte deviceId)
    {
        var request = new ChangePointerDeviceType(this._response.MajorOpcode, xAxis, yAxis, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GrabDeviceBase(uint grabWindow, uint time, GrabMode thisDeviceMode, GrabMode otherDeviceMode,
        bool ownerEvents, byte deviceId, ReadOnlySpan<uint> classes)
    {
        var request = new GrabDeviceType(this._response.MajorOpcode, grabWindow, time, (ushort)classes.Length, 
            thisDeviceMode, otherDeviceMode, ownerEvents, deviceId);
        var requestSize = request.Length * 4;
        if (requestSize < _minStackSupport)
        {
            Span<byte> scratchBuffer = stackalloc byte[requestSize];
            scratchBuffer.WriteRequest(ref request, 20, MemoryMarshal.Cast<uint, byte>(classes));
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requestSize);
            scratchBuffer[..requestSize].WriteRequest(ref request, 20, MemoryMarshal.Cast<uint, byte>(classes));
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer[..requestSize], SocketFlags.None);
        }

        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GetDeviceFocusBase(byte deviceId)
    {
        var request = new GetDeviceFocusType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GetFeedbackControlBase(byte deviceId)
    {
        var request = new GetFeedbackControlType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GetDeviceKeyMappingBase(byte deviceId, byte firstKeycode, byte count)
    {
        var request = new GetDeviceKeyMappingType(this._response.MajorOpcode, deviceId, firstKeycode, count);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GetDeviceModifierMappingBase(byte deviceId)
    {
        var request = new GetDeviceModifierMappingType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto SetDeviceModifierMappingBase(byte deviceId, ReadOnlySpan<uint> keycodesPerModifier)
    {
        var request= new SetDeviceModifierMappingType(this._response.MajorOpcode, deviceId, (byte)keycodesPerModifier.Length);
        var requestSize = request.Length * 4;
        if (requestSize < _minStackSupport)
        {
            Span<byte> scratchBuffer = stackalloc byte[requestSize];
            scratchBuffer.WriteRequest(ref request, 8, MemoryMarshal.Cast<uint, byte>(keycodesPerModifier));
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requestSize);
            scratchBuffer[..requestSize].WriteRequest(ref request, 8, MemoryMarshal.Cast<uint, byte>(keycodesPerModifier));
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer[..requestSize], SocketFlags.None);
        }

        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GetDeviceButtonMappingBase(byte deviceId)
    {
        var request = new GetDeviceButtonMappingType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto SetDeviceButtonMappingBase(byte deviceId, ReadOnlySpan<uint> map)
    {
        var request = new SetDeviceButtonMappingType(this._response.MajorOpcode, deviceId, (byte)map.Length);
        var requestSize = request.Length * 4;
        if (requestSize < _minStackSupport)
        {
            Span<byte> scratchBuffer = stackalloc byte[requestSize];
            scratchBuffer.WriteRequest(ref request, 8, MemoryMarshal.Cast<uint, byte>(map));
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requestSize);
            scratchBuffer[..requestSize].WriteRequest(ref request, 8, MemoryMarshal.Cast<uint, byte>(map));
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer[..requestSize], SocketFlags.None);
        }

        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto QueryDeviceStateBase(byte deviceId)
    {
        var request = new QueryDeviceStateType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto SetDeviceValuatorsBase(byte deviceId, byte firstValuator, ReadOnlySpan<uint> valuators)
    {
        var request = new SetDeviceValuatorsType(this._response.MajorOpcode, deviceId, firstValuator, (byte)valuators.Length);
        var requestSize = request.Length * 4;
        if (requestSize < _minStackSupport)
        {
            Span<byte> scratchBuffer = stackalloc byte[requestSize];
            scratchBuffer.WriteRequest(ref request, 20, MemoryMarshal.Cast<uint, byte>(valuators));
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requestSize);
            scratchBuffer[..requestSize].WriteRequest(ref request, 20, MemoryMarshal.Cast<uint, byte>(valuators));
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer[..requestSize], SocketFlags.None);
        }

        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GetDeviceControlBase(DeviceControl controlId, byte deviceId)
    {
        var request = new GetDeviceControlType(this._response.MajorOpcode, controlId, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto ListDevicePropertiesBase(byte deviceId)
    {
        var request = new ListDevicePropertiesType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto GetDevicePropertyBase(ATOM property, ATOM type, uint offset, uint len, byte deviceId,
        bool delete)
    {
        var request = new GetDevicePropertyType(this._response.MajorOpcode, property, type, offset, len, deviceId, 
            delete);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiQueryPointerBase(uint window, InputDevice deviceId)
    {
        var request = new XiQueryPointerType(this._response.MajorOpcode, window, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiGetClientPointerBase(uint window)
    {
        var request = new XiGetClientPointerType(this._response.MajorOpcode, window);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiQueryVersionBase(ushort majorVersion, ushort minorVersion)
    {
        var request = new XiQueryVersionType(this._response.MajorOpcode, majorVersion, minorVersion);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiQueryDeviceBase(InputDevice deviceId)
    {
        var request = new XiQueryDeviceType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiGetFocusBase(InputDevice deviceId)
    {
        var request = new XiGetFocusType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiGrabDeviceBase(uint window, uint time, uint cursor, InputDevice deviceId, GrabMode mode,
        GrabMode pairedDeviceMode, GrabOwner ownerEvents, ReadOnlySpan<uint> mask)
    {
        var request = new XiGrabDeviceType(this._response.MajorOpcode, window, time, cursor, deviceId, mode, 
            pairedDeviceMode, ownerEvents, (ushort)mask.Length);
        var requestSize = request.Length * 4;
        if (requestSize < _minStackSupport)
        {
            Span<byte> scratchBuffer = stackalloc byte[requestSize];
            scratchBuffer.WriteRequest(ref request, 24, MemoryMarshal.Cast<uint, byte>(mask));
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requestSize);
            scratchBuffer[..requestSize].WriteRequest(ref request, 24, MemoryMarshal.Cast<uint, byte>(mask));
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer[..requestSize], SocketFlags.None);
        }

        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiPassiveGrabDeviceBase(uint time, uint grabWindow, uint cursor, uint detail,
        InputDevice deviceId, GrabType grabType, GrabMode22 grabMode, GrabMode pairedDeviceMode, GrabOwner ownerEvents,
        ReadOnlySpan<uint> mask, ReadOnlySpan<uint> modifiers)
    {
        var request = new XiPassiveGrabDeviceType(this._response.MajorOpcode, time, grabWindow, cursor, detail, deviceId,
            (ushort)modifiers.Length, (ushort)mask.Length, grabType, grabMode, pairedDeviceMode, ownerEvents);
        var maskCast = MemoryMarshal.Cast<uint, byte>(mask);
        var modifiersCast = MemoryMarshal.Cast<uint, byte>(modifiers);
        var requestSize = request.Length * 4;
        if (requestSize < _minStackSupport)
        {
            Span<byte> scratchBuffer = stackalloc byte[requestSize];
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(scratchBuffer), request);
            maskCast.CopyTo(scratchBuffer[32..]);
            modifiersCast.CopyTo(scratchBuffer[(32 + maskCast.Length)..]);
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requestSize);
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(scratchBuffer[..]), request);
            maskCast.CopyTo(scratchBuffer[32..]);
            modifiersCast.CopyTo(scratchBuffer[(32 + maskCast.Length)..]);
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer[..requestSize], SocketFlags.None);
        }

        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiListPropertiesBase(InputDevice deviceId)
    {
        var request = new XiListPropertiesType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiGetPropertyBase(InputDevice deviceId, byte delete, ATOM property, ATOM type, uint offset,
        uint len)
    {
        var request = new XiGetPropertyType(this._response.MajorOpcode, deviceId, delete, property, type, offset,
            len);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto XiGetSelectedEventsBase(uint window)
    {
        var request = new XiGetSelectedEventsType(this._response.MajorOpcode, window);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }
}