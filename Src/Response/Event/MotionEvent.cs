using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct MotionNotifyEvent : IXEvent
{
    public readonly ResponseHeader<Motion> ResponseHeader;
    public uint Time;
    public uint Root;
    public uint Window;
    public uint Child;
    public short RootX;
    public short RootY;
    public short EventX;
    public short EventY;
    public ushort State;
    private sbyte _sameScreen;

    public bool IsSameScreen => _sameScreen == 1;
    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Reply == ResponseType.MotionNotify && this.ResponseHeader.Sequence == sequence;
    }
}