using System.Runtime.InteropServices;

namespace Xcsb.Models.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ClientMessageEvent
{
    public byte Format;
    public ushort Sequence;
    public uint Window;
    public uint Type;
    public ClientMessageData Data;
}