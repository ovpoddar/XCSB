using System.Runtime.InteropServices;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UnMapNotifyEvent
{
    private readonly byte Pad0;
    public ushort Sequence;
    public uint Event;
    public uint Window;
    public byte FromConfigure; // 1 true 0 false
}