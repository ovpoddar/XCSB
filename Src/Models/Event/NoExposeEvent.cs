using System.Runtime.InteropServices;

namespace Src.Models.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct NoExposeEvent
{
    private byte Pad0;
    public ushort Sequence;
    public uint Drawable;
    public ushort MinorOpcode;
    public byte MajorOpcode;
}