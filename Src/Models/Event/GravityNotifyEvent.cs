using System.Runtime.InteropServices;

namespace Xcsb.Models.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GravityNotifyEvent
{
    private readonly byte Pad0;
    public ushort Sequence;
    public uint Event;
    public uint Window;
    public short X;
    public short Y;
}