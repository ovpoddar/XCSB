using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Replies;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct AllocNamedColorReply : IXReply
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Length;
    public readonly uint Pixel;
    public readonly ushort ExactRed;
    public readonly ushort ExactGreen;
    public readonly ushort ExactBlue;
    public readonly ushort VisualRed;
    public readonly ushort VisualGreen;
    public readonly ushort VisualBlue;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Reply &&
               Length == 0;
    }
}