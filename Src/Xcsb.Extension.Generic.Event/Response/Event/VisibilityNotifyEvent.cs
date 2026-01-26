using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct VisibilityNotifyEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Window;
    public Visibility State;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.VisibilityNotify && ResponseHeader.GetValue() == 0;
    }
}