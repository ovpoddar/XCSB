using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ConfigureRequestEvent : IXEvent
{
    public readonly ResponseHeader<StackMode> ResponseHeader;
    public uint Parent;
    public uint Window;
    public uint Sibling;
    public short X;
    public short Y;
    public ushort Width;
    public ushort Height;
    public ushort BorderWidth;
    public ushort ValueMask;

    public bool Verify(in int sequence)
    {
        return (ResponseType)ResponseHeader.Reply == ResponseType.ConfigureRequest;
    }
}