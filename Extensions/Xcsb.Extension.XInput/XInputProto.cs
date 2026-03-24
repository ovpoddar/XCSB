using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Connection;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Replies;
using Xcsb.Extension.XInput.Requests;
using Xcsb.Extension.XInput.Response.Replies;

namespace Xcsb.Extension.XInput;

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
        var (result, error) = this._extensionInternal.Transport.ReceivedResponseSpan<GetExtensionVersionReply>(cookie.Id);
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
            _extensionInternal.Transport.SendRequest(scratchBuffer, SocketFlags.None);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requestSize);
            scratchBuffer[..requestSize].WriteRequest(ref request, 8, name);
            _extensionInternal.Transport.SendRequest(scratchBuffer[..requestSize], SocketFlags.None);
        }
        
        return new ResponseProto(_extensionInternal.Transport.SendSequence);
    }
    
    public void ListInputDevices()
    {
        throw new System.NotImplementedException();
    }

    public void OpenDevice()
    {
        throw new System.NotImplementedException();
    }

    public void CloseDevice()
    {
        throw new System.NotImplementedException();
    }

    public void SetDeviceMode()
    {
        throw new System.NotImplementedException();
    }

    public void SelectExtensionEvent()
    {
        throw new System.NotImplementedException();
    }

    public void GetSelectedExtensionEvents()
    {
        throw new System.NotImplementedException();
    }

    public void ChangeDeviceDontPropagateList()
    {
        throw new System.NotImplementedException();
    }

    public void GetDeviceDontPropagateList()
    {
        throw new System.NotImplementedException();
    }

    public void GetDeviceMotionEvents()
    {
        throw new System.NotImplementedException();
    }

    public void ChangeKeyboardDevice()
    {
        throw new System.NotImplementedException();
    }

    public void ChangePointerDevice()
    {
        throw new System.NotImplementedException();
    }

    public void GrabDevice()
    {
        throw new System.NotImplementedException();
    }

    public void UngrabDevice()
    {
        throw new System.NotImplementedException();
    }

    public void GrabDeviceKey()
    {
        throw new System.NotImplementedException();
    }

    public void UngrabDeviceKey()
    {
        throw new System.NotImplementedException();
    }

    public void GrabDeviceButton()
    {
        throw new System.NotImplementedException();
    }

    public void UngrabDeviceButton()
    {
        throw new System.NotImplementedException();
    }

    public void AllowDeviceEvents()
    {
        throw new System.NotImplementedException();
    }

    public void GetDeviceFocus()
    {
        throw new System.NotImplementedException();
    }

    public void SetDeviceFocus()
    {
        throw new System.NotImplementedException();
    }

    public void GetFeedbackControl()
    {
        throw new System.NotImplementedException();
    }

    public void ChangeFeedbackControl()
    {
        throw new System.NotImplementedException();
    }

    public void GetDeviceKeyMapping()
    {
        throw new System.NotImplementedException();
    }

    public void ChangeDeviceKeyMapping()
    {
        throw new System.NotImplementedException();
    }

    public void GetDeviceModifierMapping()
    {
        throw new System.NotImplementedException();
    }

    public void SetDeviceModifierMapping()
    {
        throw new System.NotImplementedException();
    }

    public void GetDeviceButtonMapping()
    {
        throw new System.NotImplementedException();
    }

    public void SetDeviceButtonMapping()
    {
        throw new System.NotImplementedException();
    }

    public void QueryDeviceState()
    {
        throw new System.NotImplementedException();
    }

    public void SendExtensionEvent()
    {
        throw new System.NotImplementedException();
    }

    public void DeviceBell()
    {
        throw new System.NotImplementedException();
    }

    public void SetDeviceValuators()
    {
        throw new System.NotImplementedException();
    }

    public void GetDeviceControl()
    {
        throw new System.NotImplementedException();
    }

    public void ChangeDeviceControl()
    {
        throw new System.NotImplementedException();
    }

    public void ListDeviceProperties()
    {
        throw new System.NotImplementedException();
    }

    public void ChangeDeviceProperty()
    {
        throw new System.NotImplementedException();
    }

    public void DeleteDeviceProperty()
    {
        throw new System.NotImplementedException();
    }

    public void GetDeviceProperty()
    {
        throw new System.NotImplementedException();
    }

    public void XIQueryPointer()
    {
        throw new System.NotImplementedException();
    }

    public void XIWarpPointer()
    {
        throw new System.NotImplementedException();
    }

    public void XIChangeCursor()
    {
        throw new System.NotImplementedException();
    }

    public void XIChangeHierarchy()
    {
        throw new System.NotImplementedException();
    }

    public void XISetClientPointer()
    {
        throw new System.NotImplementedException();
    }

    public void XIGetClientPointer()
    {
        throw new System.NotImplementedException();
    }

    public void XISelectEvents()
    {
        throw new System.NotImplementedException();
    }

    public void XIQueryVersion()
    {
        throw new System.NotImplementedException();
    }

    public void XIQueryDevice()
    {
        throw new System.NotImplementedException();
    }

    public void XISetFocus()
    {
        throw new System.NotImplementedException();
    }

    public void XIGetFocus()
    {
        throw new System.NotImplementedException();
    }

    public void XIGrabDevice()
    {
        throw new System.NotImplementedException();
    }

    public void XIUngrabDevice()
    {
        throw new System.NotImplementedException();
    }

    public void XIAllowEvents()
    {
        throw new System.NotImplementedException();
    }

    public void XIPassiveGrabDevice()
    {
        throw new System.NotImplementedException();
    }

    public void XIPassiveUngrabDevice()
    {
        throw new System.NotImplementedException();
    }

    public void XIListProperties()
    {
        throw new System.NotImplementedException();
    }

    public void XIChangeProperty()
    {
        throw new System.NotImplementedException();
    }

    public void XIDeleteProperty()
    {
        throw new System.NotImplementedException();
    }

    public void XIGetProperty()
    {
        throw new System.NotImplementedException();
    }

    public void XIGetSelectedEvents()
    {
        throw new System.NotImplementedException();
    }

    public void XIBarrierReleasePointer()
    {
        throw new System.NotImplementedException();
    }
}