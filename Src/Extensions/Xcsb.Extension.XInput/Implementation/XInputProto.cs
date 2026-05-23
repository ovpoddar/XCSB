using System;
using System.Net.Sockets;
using Xcsb.Connection;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Replies;
using Xcsb.Extension.XInput.Infrastructure;
using Xcsb.Extension.XInput.Requests;
using Xcsb.Extension.XInput.Response.Replies;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Implementation;

internal sealed class XInputProto : IXinputRequest
{
    private readonly QueryExtensionReply _response;
    private readonly IXExtensionInternal _extensionInternal;
    const int _minStackSupport = 512;

    public XInputProto(QueryExtensionReply response, IXExtensionInternal extensionInternal)
    {
        _response = response;
        _extensionInternal = extensionInternal;
    }

    public GetExtensionVersionReply GetExtensionVersion(ReadOnlySpan<byte> name)
    {
        var cookie = GetExtensionVersionBase(name);
        var (result, error) = this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetExtensionVersionReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().ToStruct<GetExtensionVersionReply>();
    }

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
    

    public ResponseProto CloseDevice(byte deviceId)
    {
        var request = new CloseDeviceType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto SelectExtensionEvent(uint window, ushort numClasses)
    {
        throw new NotImplementedException();
    }

    public ResponseProto ChangeDeviceDontPropagateList(uint window, ushort numClasses, byte mode)
    {
        throw new NotImplementedException();
    }

    public ResponseProto UngrabDevice(uint time, byte deviceId)
    {
        var request = new UngrabDeviceType(this._response.MajorOpcode, time, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto GrabDeviceKey(uint grabWindow, ushort numClasses, ushort modifiers, byte modifierDevice,
        byte grabbedDevice, byte key, byte thisDeviceMode, byte otherDeviceMode, byte ownerEvents)
    {
        throw new NotImplementedException();
    }

    public ResponseProto UngrabDeviceKey(uint grabWindow, ushort modifiers, byte modifierDevice, byte key, byte grabbedDevice)
    {
        throw new NotImplementedException();
    }

    public ResponseProto GrabDeviceButton(uint grabWindow, byte grabbedDevice, byte modifierDevice, ushort numClasses,
        ushort modifiers, byte thisDeviceMode, byte otherDeviceMode, byte button, byte ownerEvents)
    {
        throw new NotImplementedException();
    }

    public ResponseProto UngrabDeviceButton(uint grabWindow, ushort modifiers, byte modifierDevice, byte button,
        byte grabbedDevice)
    {
        throw new NotImplementedException();
    }

    public ResponseProto AllowDeviceEvents(uint time, byte mode, byte deviceId)
    {
        throw new NotImplementedException();
    }

    public ResponseProto SetDeviceFocus(uint focus, uint time, byte revertTo, byte deviceId)
    {
        throw new NotImplementedException();
    }

    public ResponseProto ChangeFeedbackControl(uint mask, byte deviceId, byte feedbackId)
    {
        throw new NotImplementedException();
    }

    public ResponseProto ChangeDeviceKeyMapping(byte deviceId, byte firstKeycode, byte keysymsPerKeycode, byte keycodeCount)
    {
        throw new NotImplementedException();
    }

    public ResponseProto DeviceBell(byte deviceId, byte feedbackId, byte feedbackClass, sbyte percent)
    {
        throw new NotImplementedException();
    }

    public ResponseProto ChangeDeviceProperty(ATOM property, ATOM type, byte deviceId, byte format, byte mode, uint numItems)
    {
        throw new NotImplementedException();
    }

    public ResponseProto DeleteDeviceProperty(ATOM property, byte deviceId)
    {
        throw new NotImplementedException();
    }

    public ResponseProto XiWarpPointer(uint srcWin, uint dstWin, int srcX, int srcY, ushort srcWidth, ushort srcHeight, int dstX,
        int dstY, ushort deviceId)
    {
        throw new NotImplementedException();
    }

    public ResponseProto XiChangeCursor(uint window, uint cursor, ushort deviceId)
    {
        throw new NotImplementedException();
    }

    public ResponseProto XiChangeHierarchy(byte numChanges)
    {
        throw new NotImplementedException();
    }

    public ResponseProto XiSetClientPointer(uint window, ushort deviceId)
    {
        throw new NotImplementedException();
    }

    public ResponseProto XiSelectEvents(uint window, ushort numMask)
    {
        throw new NotImplementedException();
    }

    public ResponseProto XiSetFocus(uint window, uint time, ushort deviceId)
    {
        throw new NotImplementedException();
    }

    public ResponseProto XiUngrabDevice(uint time, ushort deviceId)
    {
        throw new NotImplementedException();
    }

    public ResponseProto XiAllowEvents(uint time, ushort deviceId, byte eventMode, uint touchid, uint grabWindow)
    {
        throw new NotImplementedException();
    }

    public ResponseProto XiPassiveUngrabDevice(uint grabWindow, uint detail, ushort deviceId, ushort numModifiers, byte grabType)
    {
        throw new NotImplementedException();
    }

    public ResponseProto XiChangeProperty(ushort deviceId, byte mode, byte format, ATOM property, ATOM type, uint numItems)
    {
        throw new NotImplementedException();
    }

    public ResponseProto XiDeleteProperty(ushort deviceId, ATOM property)
    {
        throw new NotImplementedException();
    }

    public ResponseProto XiBarrierReleasePointer(uint numBarriers)
    {
        throw new NotImplementedException();
    }

    public ResponseProto SendExtensionEvent(uint destination, byte deviceId, byte propagate, ushort numClasses, byte numEvents)
    {
        throw new NotImplementedException();
    }

    public ChangeDeviceControlReply ChangeDeviceControl(ushort controlId, byte deviceId)
    {
        throw new NotImplementedException();
    }
}