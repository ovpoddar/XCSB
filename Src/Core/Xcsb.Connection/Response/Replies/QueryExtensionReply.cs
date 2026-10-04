using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Connection.Response.Replies;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct QueryExtensionReply : IXReply<QueryExtensionReply, QueryExtensionReply>, IVerify
{
    public readonly ResponseHeader<byte, byte> ResponseHeader;
    public readonly uint Length;
    private readonly byte _present;
    public readonly byte MajorOpcode;
    public readonly byte FirstEvent;
    public readonly byte FirstError;
    public bool Present => _present == 1;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == 1 &&
               Length == 0;
    }

    public QueryExtensionReply FromBytes(byte[] response)
    {
        return response.AsSpan().ToStruct<QueryExtensionReply>();
    }
}