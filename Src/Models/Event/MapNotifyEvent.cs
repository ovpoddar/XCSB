using System.Runtime.InteropServices;

namespace Xcsb.Models.Event;

[StructLayout(LayoutKind.Sequential)]
public struct MapNotifyEvent
{
    private byte Pad0;
    public ushort Sequence;
    public uint Event;
    public uint Window;
    public bool OverrideRedirect;
}
