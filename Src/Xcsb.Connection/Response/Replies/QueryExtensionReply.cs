using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Connection.Response.Replies;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct QueryExtensionReply : IXReply
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Length;
    private readonly byte _present;
    public readonly byte MajorOpcode;
    public readonly byte FirstEvent;
    public readonly byte FirstError;
    public bool Present => _present == 1;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.GetResponseType() == XResponseType.Reply &&
               Length == 0;
    }
}

internal struct ExtensationDetails
{
    public ResponseHeader<byte> ResponseHeader;
    public byte MajorOpcode;
    public byte FirstEvent;
    public byte FirstError;
    public int EventLenght;
    public int ErrorLenght;
}