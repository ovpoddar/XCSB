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
    public sbyte SameScreen; // TODO 1 true 0 false

    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Sequence == sequence;
    }
}