using System.Runtime.InteropServices;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ClientMessageEvent
{
    public byte Format;
    public ushort Sequence;
    public uint Window;
    public uint Type;
    public ClientMessageData Data;
}