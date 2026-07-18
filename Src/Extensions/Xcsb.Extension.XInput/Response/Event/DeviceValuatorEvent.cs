using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public unsafe struct DeviceValuatorEvent : IXEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly ushort DeviceState;
    public readonly byte NumValuators;
    public readonly byte FirstValuator;
    private fixed int _valuator[6];

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }

    public readonly Span<int> Valuator
    {
        get
        {
            fixed (int* ptr = this._valuator)
                return new Span<int>(ptr, 32);
        }
    }
}