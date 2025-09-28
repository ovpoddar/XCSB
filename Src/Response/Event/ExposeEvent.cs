using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ExposeEvent :  IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Window;
    public ushort X;
    public ushort Y;
    public ushort Width;
    public ushort Height;
    public ushort Count;


    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Reply == ResponseType.Expose && this.ResponseHeader.GetValue() == 0;
    }
}