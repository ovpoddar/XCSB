using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Replies.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct GetFontPathResponse : IXReply
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Length;
    public readonly ushort StringLength;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Reply &&
               Length != StringLength;
    }
}