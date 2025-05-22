using System.Runtime.InteropServices;

namespace Src.Models.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UnMapNotifyEvent
{
    private byte Pad0;
    public ushort Sequence;
    public uint Event;
    public uint Window;
    public byte FromConfigure; // 1 true 0 false
}