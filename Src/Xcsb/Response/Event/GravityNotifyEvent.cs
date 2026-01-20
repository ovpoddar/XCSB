using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct GravityNotifyEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Event;
    public uint Window;
    public short X;
    public short Y;


    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.GravityNotify && ResponseHeader.GetValue() == 0;
    }
}