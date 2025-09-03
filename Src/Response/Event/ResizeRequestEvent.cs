using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ResizeRequestEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Window;
    public ushort Width;
    public ushort Height;

    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Reply == ResponseType.ResizeRequest && this.ResponseHeader.GetValue() == 0;
    }
}