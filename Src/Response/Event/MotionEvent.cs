using System.Runtime.InteropServices;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct MotionNotifyEvent
{
    public Motion Detail;
    public ushort Sequence;
    public uint Time;
    public uint Root;
    public uint Window;
    public uint Child;
    public short RootX;
    public short RootY;
    public short EventX;
    public short EventY;
    public ushort State;
    public sbyte SameScreen; // 1 true 0 false
}