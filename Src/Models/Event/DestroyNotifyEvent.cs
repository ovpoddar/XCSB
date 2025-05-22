using System.Runtime.InteropServices;

namespace Src.Models.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct DestroyNotifyEvent
{
    private byte Pad0;
    public ushort Sequence;
    public uint Event;
    public uint Window;
}