using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct MapNotifyEvent : IXEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Event;
    public uint Window;
    public bool OverrideRedirect;

    public readonly bool Verify()
    {
        return ResponseHeader.Reply == ResponseType.MapNotify
            && ResponseHeader.GetValue() == 0;
    }
}