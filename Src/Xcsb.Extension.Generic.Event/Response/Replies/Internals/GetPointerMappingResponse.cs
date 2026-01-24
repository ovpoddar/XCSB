using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Replies.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct GetPointerMappingResponse : IXReply
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Length;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Reply && ResponseHeader.Sequence == sequence;
    }

    public byte MapLength => ResponseHeader.GetValue();
}