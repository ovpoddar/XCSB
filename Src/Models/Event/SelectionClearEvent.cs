using System.Runtime.InteropServices;

namespace Xcsb.Models.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SelectionClearEvent
{
    private byte Pad0;
    public ushort Sequence;
    public uint Time;
    public uint Owner;
    public uint Selection;
}