using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Replies.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 60)]
internal readonly struct ListFontsWithInfoResponse : IXReply
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Length;
    public readonly CharInfo MinBounds;
    private readonly int _pad0;
    public readonly CharInfo MaxBounds;
    private readonly int _pad1;
    public readonly ushort MinChar;
    public readonly ushort MaxChar;
    public readonly ushort DefaultChar;
    public readonly ushort PropertiLenght;
    public readonly FontDraw Direction;
    public readonly byte MinByte;
    public readonly byte MaxByte;
    public readonly byte AllCharsExist;
    public readonly ushort FontAscent;
    public readonly ushort FontDescent;
    public readonly uint ReplyHint;

    public bool Verify(in int sequence)
    {
        return (ResponseType)ResponseHeader.Reply == ResponseType.Reply &&
               _pad0 == 0 && _pad1 == 0;
    }

    public byte NameLength =>
        ResponseHeader.GetValue();

    internal bool HasMore =>
        NameLength != 0;
}