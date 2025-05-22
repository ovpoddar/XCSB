using System.Runtime.InteropServices;

namespace Src.Models.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct InputEvent
{
    public byte Detail;
    public ushort SequenceNumber;
    public uint TimeStamp;
    public uint RootWindow;
    public uint EventWindow;
    public uint ChildWindow;
    public short RootX;
    public short RootY;
    public short EventX;
    public short EventY;
    public KeyButMask State;
    public sbyte IsSameScreen; // 1 true, 0 false
}