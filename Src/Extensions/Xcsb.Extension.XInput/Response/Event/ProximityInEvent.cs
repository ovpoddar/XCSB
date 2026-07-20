using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;
using Xcsb.Masks;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct ProximityInEvent : IXEvent
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

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }

    public bool SameScreen => this._sameScreen == 1;
}