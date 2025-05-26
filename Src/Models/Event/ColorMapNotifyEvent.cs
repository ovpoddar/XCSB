using System.Runtime.InteropServices;

namespace Xcsb.Models.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ColorMapNotifyEvent
{
    private byte Pad0;
    public ushort Sequence;
    public uint Window;
    public uint ColorMap;
    public byte New;
    public ColormapState State;
}