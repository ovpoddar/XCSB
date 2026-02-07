using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct NoExposeEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Drawable;
    public ushort MinorOpcode;
    public byte MajorOpcode;

    public readonly bool Verify()
    {
        return (ResponseType)ResponseHeader.Reply == ResponseType.NoExpose && ResponseHeader.GetValue() == 0;
    }
}