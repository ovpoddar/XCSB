using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct DestroyNotifyEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Event;
    public uint Window;


    public bool Verify()
    {
        return (ResponseType)ResponseHeader.Reply == ResponseType.DestroyNotify && ResponseHeader.GetValue() == 0;
    }
}