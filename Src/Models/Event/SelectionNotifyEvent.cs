using System.Runtime.InteropServices;

namespace Xcsb.Models.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SelectionNotifyEvent
{
    private byte Pad0;
    public ushort Sequence;
    public uint Time;
    public uint Requestor;
    public uint Selection;
    public uint Target;
    public uint Property;
}