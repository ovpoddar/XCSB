using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Replies.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct GetPropertyResponse : IXReply
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Length;
    public readonly uint Type;
    public readonly uint BytesAfter;
    public readonly uint ValueLength;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Reply &&
               ValueLength != Length;
    }

    public byte Format => ResponseHeader.GetValue();
}