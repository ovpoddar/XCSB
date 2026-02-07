using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct MappingNotifyEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public Mapping Request;
    public byte FirstKeyCode;
    public byte Count;

    public readonly bool Verify()
    {
        return (ResponseType)ResponseHeader.Reply == ResponseType.MappingNotify && ResponseHeader.GetValue() == 0;
    }
}