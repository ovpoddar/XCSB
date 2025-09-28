using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct PropertyNotifyEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Window;
    public uint Atom;
    public uint Time;
    public NotifyState State;

    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Reply == ResponseType.PropertyNotify
            && this.ResponseHeader.GetValue() == 0;
    }
}