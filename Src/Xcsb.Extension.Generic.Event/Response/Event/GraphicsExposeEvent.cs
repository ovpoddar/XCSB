using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct GraphicsExposeEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Drawable;
    public ushort X;
    public ushort Y;
    public ushort Width;
    public ushort Height;
    public ushort MinorOpcode;
    public ushort Count;
    public byte MajorOpcode;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.GraphicsExpose && ResponseHeader.GetValue() == 0;
    }
}