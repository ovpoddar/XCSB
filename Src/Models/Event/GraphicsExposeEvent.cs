using System.Runtime.InteropServices;

namespace Src.Models.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GraphicsExposeEvent
{
    public EventType EventType; // 13
    public byte Pad0;
    public ushort Sequence;
    public uint Drawable;
    public ushort X;
    public ushort Y;
    public ushort Width;
    public ushort Height;
    public ushort MinorOpcode;
    public ushort Count;
    public byte MajorOpcode;
}