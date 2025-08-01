using System.Runtime.InteropServices;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SelectionRequestEvent
{
    private readonly byte Pad0;
    public ushort Sequence;
    public uint Time; // 0 -> current time
    public uint Owner;
    public uint Requestor;
    public uint Selection;
    public uint Target;

    public uint Property; // has some fixed value but assuming can have more than that so not adding them
    // if its less than 68 it map to some value 
}