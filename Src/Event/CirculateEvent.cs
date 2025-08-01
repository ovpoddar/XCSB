using System.Runtime.InteropServices;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CirculateEvent
{
    private readonly byte Pad0;
    public ushort Sequence;
    public uint Event;
    public uint Window;
    public int Pad1;
    public Place Place;
}