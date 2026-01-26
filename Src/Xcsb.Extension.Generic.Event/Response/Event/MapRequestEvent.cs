using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct MapRequestEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Parent;
    public uint Window;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.MapRequest && ResponseHeader.GetValue() == 0;
    }
}