using System.Runtime.InteropServices;

namespace Xcsb.Models.Event;

[StructLayout(LayoutKind.Sequential)]
public struct MapRequestEvent
{
    private readonly byte Pad0;
    public ushort Sequence;
    public uint Parent;
    public uint Window;
}
