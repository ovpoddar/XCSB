using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 60)]
internal readonly struct ListFontsWithInfoResponse : IXBaseResponse
{
    public readonly byte Reply;
    public readonly byte NameLength;
    public readonly ushort Sequence;
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
    public bool Verify()
    {
        return this.Reply == 1 && this._pad0 == 0 && this._pad1 == 0;
    }
}