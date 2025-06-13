using System.Runtime.InteropServices;

namespace Xcsb.Models.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ResizeRequestEvent
{
    private byte Pad0;
    public ushort Sequence;
    public uint Window;
    public ushort Width;
    public ushort Height;
}