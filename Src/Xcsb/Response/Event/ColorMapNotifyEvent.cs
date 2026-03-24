using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ColorMapNotifyEvent : IXEvent
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Window;
    public uint ColorMap;
    public byte New;
    public ColormapState State;


    public readonly bool Verify()
    {
        return ResponseHeader.Reply == ResponseType.ColormapNotify;
    }
}