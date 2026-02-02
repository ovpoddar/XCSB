using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Connection.Response.Replies.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct ListExtensionsResponse : IXReply
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Length;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.GetResponseType() == XResponseType.Reply &&
               Length * 4 >= NumberOfExtensions;
    }

    public byte NumberOfExtensions => ResponseHeader.GetValue();
}