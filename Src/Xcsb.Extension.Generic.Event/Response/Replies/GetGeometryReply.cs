using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Replies;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GetGeometryReply : IXReply
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Length;
    public readonly uint Root;
    public readonly ushort X;
    public readonly ushort Y;
    public readonly ushort Width;
    public readonly ushort Height;
    public readonly ushort BorderWidth;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Reply &&
               Length == 0;
    }

    public byte Depth => ResponseHeader.GetValue();
}