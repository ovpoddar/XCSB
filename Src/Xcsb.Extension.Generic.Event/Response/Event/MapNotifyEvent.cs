using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct MapNotifyEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Event;
    public uint Window;
    public bool OverrideRedirect;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.MapNotify
            && ResponseHeader.GetValue() == 0;
    }
}