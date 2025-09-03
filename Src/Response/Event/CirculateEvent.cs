using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct CirculateNotifyEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Event;
    public uint Window;
    private readonly uint _pad1;
    public Place Place;
    
    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Reply == ResponseType.CirculateNotify && this.ResponseHeader.GetValue() == 0 && this._pad1 == 0;
    }
}
