using System.Runtime.InteropServices;

namespace Xcsb.Models.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct NoExposeEvent
{
    private readonly byte Pad0;
    public ushort Sequence;
    public uint Drawable;
    public ushort MinorOpcode;
    public byte MajorOpcode;
}