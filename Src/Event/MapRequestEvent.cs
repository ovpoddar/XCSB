using System.Runtime.InteropServices;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential)]
public struct MapRequestEvent
{
    private readonly byte Pad0;
    public ushort Sequence;
    public uint Parent;
    public uint Window;
}