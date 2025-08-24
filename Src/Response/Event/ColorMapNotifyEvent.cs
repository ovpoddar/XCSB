using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ColorMapNotifyEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Window;
    public uint ColorMap;
    public byte New;
    public ColormapState State;


    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Reply == ResponseType.ColormapNotify && this.ResponseHeader.Sequence == sequence;
    }
}