using System.Runtime.InteropServices;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct QueryTextExtentsReply : IXBaseResponse
{
    public readonly byte Reply;
    public readonly FontDraw FontDraw;
    public readonly ushort Sequence;
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
        return Reply == 1 && Length == 0 && Sequence == sequence;
    }
}