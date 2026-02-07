using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

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

    public readonly bool Verify()
    {
        return (ResponseType)ResponseHeader.Reply == ResponseType.GraphicsExpose && ResponseHeader.GetValue() == 0;
    }
}