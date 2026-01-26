using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct CirculateRequestEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Parent;
    public uint Window;
    private readonly uint _pad1;
    public Place Place;


    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.CirculateRequest && ResponseHeader.GetValue() == 0 && _pad1 == 0;
    }
}