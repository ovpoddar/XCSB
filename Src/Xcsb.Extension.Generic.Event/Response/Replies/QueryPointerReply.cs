using System.Runtime.InteropServices;
using Xcsb.Response.Contract;
using Xcsb.Response.Event;

namespace Xcsb.Extension.Generic.Event.Response.Replies;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct QueryPointerReply : IXReply
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Length;
    public readonly uint Root;
    public readonly uint child;
    public readonly short RootX;
    public readonly short RootY;
    public readonly short WinX;
    public readonly short WinY;
    public readonly KeyButMask Mask;

    public readonly bool IsSameScreen => ResponseHeader.GetValue() == 1;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Reply &&
               Length == 0;
    }
}