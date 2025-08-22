using System.Runtime.InteropServices;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential)]
public struct ReParentNotifyEvent
{
    private readonly byte Pad0;
    public ushort Sequence;
    public uint Event;
    public uint Window;
    public uint Parent;
    public short X;
    public short Y;
    public bool OverrideRedirect;
}