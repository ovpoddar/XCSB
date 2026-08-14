using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.TypeInfo;
using Xcsb.Masks;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct DeviceButtonPressEvent : IXEvent<DeviceButtonPressEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint TimeStamp;
    public readonly uint RootWindow;
    public readonly uint Event;
    public readonly uint Child;
    public readonly short RootX;
    public readonly short RootY;
    public readonly short EventX;
    public readonly short EventY;
    public readonly KeyButMask State;
    private readonly byte _sameScreen;
    public readonly byte DeviceId;

    public bool SameScreen => this._sameScreen == 1;
    public ref readonly DeviceButtonPressEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<DeviceButtonPressEvent>();
        if ((byte)result.ResponseHeader.Reply == (byte)XiInputEventType.DeviceButtonPress || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
        
    }
}