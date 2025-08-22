using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ClientMessageEvent
{
    public byte Format;
    public ushort Sequence;
    public uint Window;
    public ATOM Type;
    public ClientMessageData Data;
}