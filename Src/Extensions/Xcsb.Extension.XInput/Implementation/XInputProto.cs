using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Replies;
using Xcsb.Extension.XInput.Infrastructure;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.Writers;
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
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetExtensionVersionReply>(cookie.Id);
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

    public ResponseProto SelectExtensionEvent(uint window, ushort numClasses, uint[] foo)
    {
        throw new NotImplementedException();
    }

    public ResponseProto ChangeDeviceDontPropagateList(uint window, ushort numClasses, byte mode, uint[] foo)
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
        byte grabbedDevice, byte key, byte thisDeviceMode, byte otherDeviceMode, byte ownerEvents, uint[] foo)
    {
        throw new NotImplementedException();
    }

    public ResponseProto UngrabDeviceKey(uint grabWindow, ushort modifiers, byte modifierDevice, byte key,
        byte grabbedDevice)
    {
        var request = new UngrabDeviceKeyType(this._response.MajorOpcode, grabWindow, modifiers, modifierDevice, key,
            grabbedDevice);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto GrabDeviceButton(uint grabWindow, byte grabbedDevice, byte modifierDevice, ushort numClasses,
        ushort modifiers, byte thisDeviceMode, byte otherDeviceMode, byte button, byte ownerEvents, uint[] foo)
    {
        throw new NotImplementedException();
    }

    public ResponseProto UngrabDeviceButton(uint grabWindow, ushort modifiers, byte modifierDevice, byte button,
        byte grabbedDevice)
    {
        var request = new UngrabDeviceButtonType(this._response.MajorOpcode, grabWindow, modifiers, modifierDevice,
            button, grabbedDevice);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto AllowDeviceEvents(uint time, byte mode, byte deviceId)
    {
        var request = new AllowDeviceEventsType(this._response.MajorOpcode, time, mode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto SetDeviceFocus(uint focus, uint time, byte revertTo, byte deviceId)
    {
        var request = new SetDeviceFocusType(this._response.MajorOpcode, focus, time, revertTo, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto ChangeFeedbackControl<T>(FeedbackControlMask mask, byte deviceId, byte feedbackId, T feedback)
        where T : IFeedback
    {
        var request = new ChangeFeedbackControlType(this._response.MajorOpcode, mask, deviceId, feedbackId,
            feedback.Length);
        var requestSize = request.Length * 4;
        Span<byte> scratchBuffer = stackalloc byte[requestSize];
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(scratchBuffer), request);
        if (feedback is not StringFeedback fb)
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(scratchBuffer[12..]), feedback);
        else
        {
            var feedbackHead = fb.m_feedback;
            scratchBuffer[12..].WriteRequest(
                ref feedbackHead,
                8,
                MemoryMarshal.Cast<uint, byte>(fb.m_keysyms)
            );
        }
        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto ChangeDeviceKeyMapping(byte deviceId, byte firstKeycode, byte keysymsPerKeycode,
        byte keycodeCount,
        uint[] foo)
    {
        throw new NotImplementedException();
    }

    public ResponseProto DeviceBell(byte deviceId, byte feedbackId, byte feedbackClass, sbyte percent)
    {
        var request = new DeviceBellType(this._response.MajorOpcode, deviceId, feedbackId, feedbackClass, percent);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto ChangeDeviceProperty(ATOM property, ATOM type, byte deviceId, byte format, byte mode,
        uint numItems,
        byte[] foo)
    {
        throw new NotImplementedException();
    }

    public ResponseProto DeleteDeviceProperty(ATOM property, byte deviceId)
    {
        var request = new DeleteDevicePropertyType(this._response.MajorOpcode, property, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiWarpPointer(uint srcWin, uint dstWin, int srcX, int srcY, ushort srcWidth, ushort srcHeight,
        int dstX, int dstY, ushort deviceId)
    {
        var request = new XiWarpPointerType(this._response.MajorOpcode, srcWin, dstWin, srcX, srcY, srcWidth, srcHeight,
            dstX, dstY, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiChangeCursor(uint window, uint cursor, ushort deviceId)
    {
        var request = new XiChangeCursorType(this._response.MajorOpcode, window, cursor, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiChangeHierarchy(HierarchyChangeBuilder builder)
    {
        var request = new XiChangeHierarchyType(this._response.MajorOpcode, (byte)builder.m_length, 
            builder.m_data.Length / 4);
        Span<byte> scratchBuffer = stackalloc byte[request.Length * 4];
        scratchBuffer.WriteRequest(
            ref request,
            8,
            builder.m_data);
        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiSetClientPointer(uint window, ushort deviceId)
    {
        var request = new XiSetClientPointerType(this._response.MajorOpcode, window, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiSelectEvents(uint window, EventMaskBuilder mask)
    {
        var request = new XiSelectEventsType(this._response.MajorOpcode, window, (ushort)mask._length);
        Span<byte> scratchBuffer = stackalloc byte[request.Length * 4];
        scratchBuffer.WriteRequest(
            ref request,
            12,
            MemoryMarshal.Cast<uint, byte>(mask._data));
        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiSetFocus(uint window, uint time, ushort deviceId)
    {
        var request = new XiSetFocusType(this._response.MajorOpcode, window, time, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiUngrabDevice(uint time, ushort deviceId)
    {
        var request = new XiUngrabDeviceType(this._response.MajorOpcode, time, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiAllowEvents(uint time, ushort deviceId, byte eventMode, uint touchId, uint grabWindow)
    {
        var request = new XiAllowEventsType(this._response.MajorOpcode, time, deviceId, eventMode, touchId, grabWindow);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiPassiveUngrabDevice(uint grabWindow, uint detail, ushort deviceId, ushort numModifiers,
        byte grabType, uint[] foo)
    {
        throw new NotImplementedException();
    }

    public ResponseProto XiChangeProperty(ushort deviceId, byte mode, byte format, ATOM property, ATOM type,
        uint numItems, byte[] foo)
    {
        throw new NotImplementedException();
    }

    public ResponseProto XiDeleteProperty(ushort deviceId, ATOM property)
    {
        var request = new XiDeletePropertyType(this._response.MajorOpcode, deviceId, property);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto XiBarrierReleasePointer(ReadOnlySpan<BarrierReleasePointerInfo> barriers)
    {
        var request = new XiBarrierReleasePointerType(this._response.MajorOpcode, (uint)barriers.Length);
        Span<byte> scratchBuffer = stackalloc byte[request.Length * 4];
        scratchBuffer.WriteRequest(
            ref request,
            8,
            MemoryMarshal.Cast<BarrierReleasePointerInfo, byte>(barriers));
        _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto SendExtensionEvent(uint destination, byte deviceId, byte propagate, ushort numClasses,
        byte numEvents, int[] foo)
    {
        throw new NotImplementedException();
    }

    public ChangeDeviceControlReply ChangeDeviceControl(ushort controlId, byte deviceId)
    {
        var request = new ChangeDeviceControlType(this._response.MajorOpcode, controlId, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        
    }
}