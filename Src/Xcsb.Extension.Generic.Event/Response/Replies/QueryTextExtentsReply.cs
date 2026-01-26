using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;
using Xcsb.Extension.Generic.Event.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Replies;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct QueryTextExtentsReply : IXReply
{
    public readonly ResponseHeader<FontDraw> ResponseHeader;
    public readonly uint Length;
    public readonly ushort FontAscent;
    public readonly ushort FontDescent;
    public readonly ushort OverallAscent;
    public readonly ushort OverallDescent;
    public readonly uint OverallWidth;
    public readonly uint OverallLeft;
    public readonly uint OverallRight;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Reply &&
               Length == 0;
    }

    public readonly FontDraw FontDraw => ResponseHeader.GetValue();
}