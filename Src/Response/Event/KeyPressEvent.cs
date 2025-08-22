using System.Runtime.InteropServices;

namespace Xcsb.Event;

// TODO: need a way to access similar event in a single type
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct KeyPressEvent
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