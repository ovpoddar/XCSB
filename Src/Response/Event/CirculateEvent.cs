using System.Runtime.InteropServices;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CirculateNotifyEvent
{
    private readonly byte _pad0;
    public ushort Sequence;
    public uint Event;
    public uint Window;
    private readonly uint _pad1;
    public Place Place;
}
