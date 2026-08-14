using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.TypeInfo;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public unsafe struct DeviceValuatorEvent : IXEvent<DeviceValuatorEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly ushort DeviceState;
    public readonly byte NumValuators;
    public readonly byte FirstValuator;
    private fixed int _valuator[6];

    public readonly Span<int> Valuator
    {
        get
        {
            fixed (int* ptr = this._valuator)
                return new Span<int>(ptr, 32);
        }
    }

    public ref readonly DeviceValuatorEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<DeviceValuatorEvent>();
        if ((byte)result.ResponseHeader.Reply == (byte)XiInputEventType.DeviceValuator || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}