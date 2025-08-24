using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

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
    public byte OverrideRedirect; // TODO 1 true 0 false


    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Sequence == sequence && this.ResponseHeader.GetValue() == 0;
    }
}