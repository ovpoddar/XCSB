using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

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
        return this.ResponseHeader.Reply == ResponseType.ConfigureNotify && this.ResponseHeader.Sequence == sequence && this.ResponseHeader.GetValue() == 0;
    }
}