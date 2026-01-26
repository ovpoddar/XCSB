using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct EnterNotifyEvent : IXEvent
{
    public readonly ResponseHeader<NotifyDetail> ResponseHeader;
    public uint Time;
    public uint Root;
    public uint Event;
    public uint Child;
    public short RootX;
    public short RootY;
    public short EventX;
    public short EventY;
    public ushort State;
    public NotifyMode Mode;
    private byte _sameScreenFocus;

    public bool IsSameScreenFocus => _sameScreenFocus == 1;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.EnterNotify;
    }
}