using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct MapRequestEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Parent;
    public uint Window;

    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Reply == ResponseType.MapRequest && this.ResponseHeader.Sequence == sequence && this.ResponseHeader.GetValue() == 0;
    }
}