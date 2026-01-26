using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct FocusInEvent : IXEvent
{
    public readonly ResponseHeader<NotifyDetail> ResponseHeader;
    public int Event;
    public NotifyMode Mode;


    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.FocusIn;
    }
}