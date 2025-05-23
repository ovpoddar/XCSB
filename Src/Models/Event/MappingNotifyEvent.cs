using System.Runtime.InteropServices;

namespace Src.Models.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct MappingNotifyEvent
{
    private byte Pad0;
    public ushort Sequence;
    public Mapping Request;
    public byte FirstKeyCode;
    public byte Count;
}