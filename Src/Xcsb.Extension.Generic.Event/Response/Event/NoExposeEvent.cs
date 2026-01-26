using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct NoExposeEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Drawable;
    public ushort MinorOpcode;
    public byte MajorOpcode;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.NoExpose && ResponseHeader.GetValue() == 0;
    }
}