using System.Runtime.InteropServices;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 60)]
internal readonly struct ListFontsWithInfoResponse : IXBaseResponse
{
    public readonly ResponseHeader<byte> ResponseHeader;
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
        //NameLength
        return _pad0 == 0 && _pad1 == 0;
    }
}