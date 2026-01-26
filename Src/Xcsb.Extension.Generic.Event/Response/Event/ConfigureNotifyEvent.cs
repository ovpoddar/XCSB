using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ConfigureNotifyEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Event;
    public uint Window;
    public uint AboveSibling;
    public short X;
    public short Y;
    public ushort Width;
    public ushort Height;
    public ushort BorderWidth;
    public byte OverrideRedirect;


    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.ConfigureNotify
            && ResponseHeader.GetValue() == 0;
    }
}