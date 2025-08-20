using System.Runtime.InteropServices;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ButtonReleaseEvent
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
    private sbyte _isSameScreen;
    public bool IsSameScreen => _isSameScreen == 1;
}