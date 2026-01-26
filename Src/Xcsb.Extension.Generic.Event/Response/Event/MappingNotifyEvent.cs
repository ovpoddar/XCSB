using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct MappingNotifyEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public Mapping Request;
    public byte FirstKeyCode;
    public byte Count;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.MappingNotify && ResponseHeader.GetValue() == 0;
    }
}