using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct NoExposeEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Drawable;
    public ushort MinorOpcode;
    public byte MajorOpcode;

    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Reply == ResponseType.NoExpose && this.ResponseHeader.GetValue() == 0;
    }
}