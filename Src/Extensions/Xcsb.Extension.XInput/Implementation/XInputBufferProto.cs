using System;
using Xcsb.Connection;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Response.Replies;
using Xcsb.Extension.XInput.Infrastructure;
using Xcsb.Extension.XInput.Infrastructure.VoidProto;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.Writers;
using Xcsb.Extension.XInput.Requests;
using Xcsb.Extension.XInput.Response.Replies;
using Xcsb.Extension.XInput.Response.Replies.Internals;
using Xcsb.Masks;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Implementation;

// https://xorg.freedesktop.org/archive/X11R7.7/doc/libXi/inputlib.pdf
internal class XInputBufferProto : IXInputBufferRequest
{
    void IVoidBufferProto.CloseDevice(byte deviceId)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.SelectExtensionEvent(uint window, ReadOnlySpan<uint> classes)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.ChangeDeviceDontPropagateList(uint window, PropagateMode mode, ReadOnlySpan<uint> classes)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.UngrabDevice(uint time, byte deviceId)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.GrabDeviceKey(uint grabWindow, ModifierMask modifiers, byte modifierDevice, byte grabbedDevice, byte key,
        GrabMode thisDeviceMode, GrabMode otherDeviceMode, bool ownerEvents, ReadOnlySpan<uint> classes)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.UngrabDeviceKey(uint grabWindow, ModifierMask modifiers, byte modifierDevice, byte key, byte grabbedDevice)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.GrabDeviceButton(uint grabWindow, byte grabbedDevice, byte modifierDevice, ModifierMask modifiers,
        GrabMode thisDeviceMode, GrabMode otherDeviceMode, byte button, bool ownerEvents, ReadOnlySpan<uint> classes)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.UngrabDeviceButton(uint grabWindow, ModifierMask modifiers, byte modifierDevice, byte button, byte grabbedDevice)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.AllowDeviceEvents(uint time, DeviceInputMode mode, byte deviceId)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.SetDeviceFocus(uint focus, uint time, InputFocusMode revertTo, byte deviceId)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.ChangeFeedbackControl<T>(FeedbackControlMask mask, byte deviceId, byte feedbackId, T feedback)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.ChangeDeviceKeyMapping(byte deviceId, byte firstKeycode, byte keysymsPerKeycode, byte keycodeCount,
        ReadOnlySpan<uint> keysyms)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.DeviceBell(byte deviceId, byte feedbackId, byte feedbackClass, sbyte percent)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.ChangeDeviceProperty<T>(ATOM property, ATOM type, byte deviceId, PropertyMode mode, ReadOnlySpan<T> items)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.DeleteDeviceProperty(ATOM property, byte deviceId)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.XiWarpPointer(uint srcWin, uint dstWin, int srcX, int srcY, ushort srcWidth, ushort srcHeight, int dstX, int dstY,
        InputDevice deviceId)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.XiChangeCursor(uint window, uint cursor, InputDevice deviceId)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.XiChangeHierarchy(HierarchyChangeBuilder builder)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.XiSetClientPointer(uint window, InputDevice deviceId)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.XiSelectEvents(uint window, EventMaskBuilder mask)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.XiSetFocus(uint window, uint time, InputDevice deviceId)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.XiUngrabDevice(uint time, InputDevice deviceId)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.XiAllowEvents(uint time, InputDevice deviceId, EventMode eventMode, uint touchId, uint grabWindow)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.XiPassiveUngrabDevice(uint grabWindow, uint detail, InputDevice deviceId, GrabType grabType,
        ReadOnlySpan<uint> modifiers)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.XiChangeProperty<T>(InputDevice deviceId, PropertyMode mode, ATOM property, ATOM type, ReadOnlySpan<T> items)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.XiDeleteProperty(InputDevice deviceId, ATOM property)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.XiBarrierReleasePointer(ReadOnlySpan<BarrierReleasePointerInfo> barriers)
    {
        throw new NotImplementedException();
    }

    void IVoidBufferProto.SendExtensionEvent(uint destination, byte deviceId, bool propagate, ReadOnlySpan<int> classes, ReadOnlySpan<InputEvents> events)
    {
        throw new NotImplementedException();
    }

    public void FlushChecked()
    {
        throw new NotImplementedException();
    }

    public void Flush()
    {
        throw new NotImplementedException();
    }
}