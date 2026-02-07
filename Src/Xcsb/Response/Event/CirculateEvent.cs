using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct CirculateNotifyEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Event;
    public uint Window;
    private readonly uint _pad1;
    public Place Place;

    public readonly bool Verify()
    {
        return (ResponseType)ResponseHeader.Reply == ResponseType.CirculateNotify && ResponseHeader.GetValue() == 0 && _pad1 == 0;
    }
}
