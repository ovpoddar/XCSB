using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct CreateNotifyEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Parent;
    public uint Window;
    public short X;
    public short Y;
    public ushort Width;
    public ushort Height;
    public ushort BorderWidth;
    private byte _overrideRedirect;

    public bool OverrideRedirect => _overrideRedirect == 1;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.CreateNotify && //this.ResponseHeader.Sequence == sequence &&
               ResponseHeader.GetValue() == 0;
    }
}