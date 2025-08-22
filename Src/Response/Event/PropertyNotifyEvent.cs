using System.Runtime.InteropServices;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PropertyNotifyEvent
{
    private readonly byte Pad0;
    public ushort Sequence;
    public uint Window;
    public uint Atom;
    public uint Time;
    public NotifyState State;
}