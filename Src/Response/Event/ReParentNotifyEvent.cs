using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ReParentNotifyEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Event;
    public uint Window;
    public uint Parent;
    public short X;
    public short Y;
    public bool OverrideRedirect;
    
    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Reply == ResponseType.ReParentNotify && this.ResponseHeader.GetValue() == 0;
    }
}