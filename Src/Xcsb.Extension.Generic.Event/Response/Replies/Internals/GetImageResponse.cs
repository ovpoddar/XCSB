using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Replies.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct GetImageResponse : IXReply
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Length;
    public readonly uint VisualId;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Reply && ResponseHeader.Sequence == sequence;
    }
}