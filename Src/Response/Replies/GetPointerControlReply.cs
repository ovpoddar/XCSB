using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Replies;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GetPointerControlReply : IXReply
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Length;
    public readonly ushort AccelNumerator;
    public readonly ushort AccelDenominator;
    public readonly ushort Threshold;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Reply &&
               Length == 0;
    }
}