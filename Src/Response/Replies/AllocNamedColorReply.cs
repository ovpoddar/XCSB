using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response;

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
        return this.ResponseHeader.Reply == ResponseType.Reply && this.ResponseHeader.Sequence == sequence &&
               this.Length == 0;
    }
}