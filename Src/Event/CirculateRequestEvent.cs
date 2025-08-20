using System.Runtime.InteropServices;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CirculateRequestEvent
{
    private readonly byte _pad0;
    public ushort Sequence;
    public uint Parent;
    public uint Window;
    private readonly uint _pad1;
    public Place Place;
}