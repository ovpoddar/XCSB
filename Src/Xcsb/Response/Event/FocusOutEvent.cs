using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct FocusOutEvent : IXEvent
{
    public readonly ResponseHeader<NotifyDetail> ResponseHeader;
    public int Event;
    public NotifyMode Mode;


    public bool Verify()
    {
        return (ResponseType)ResponseHeader.Reply == ResponseType.FocusOut;// && this.ResponseHeader.Sequence == sequence;
    }
}