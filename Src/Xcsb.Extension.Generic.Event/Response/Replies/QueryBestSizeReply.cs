using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Replies;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct QueryBestSizeReply : IXReply
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Length;
    public readonly ushort Width;
    public readonly ushort Height;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Reply &&
               Length == 0;
    }
}