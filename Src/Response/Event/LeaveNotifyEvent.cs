using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct LeaveNotifyEvent : IXEvent
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
    public byte SameScreenFocus; // 1 true, 0 false

    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Reply == ResponseType.LeaveNotify;
    }
}