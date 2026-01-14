using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct UnMapNotifyEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Event;
    public uint Window;
    private byte _fromConfigure;


    public readonly bool FromConfigure => _fromConfigure == 1;
    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.UnMapNotify && ResponseHeader.GetValue() == 0;
    }
}