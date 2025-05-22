using System.Runtime.InteropServices;

namespace Src.Models.Event;

[StructLayout(LayoutKind.Sequential)]
public struct MapRequestEvent
{
    private byte Pad0;
    public ushort Sequence;
    public uint Parent;
    public uint Window;
}
