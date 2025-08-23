using System.Runtime.InteropServices;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 60)]
internal readonly struct QueryFontResponse : IXReply
{
    public readonly ResponseHeader<byte>ResponseHeader;
    public readonly uint Length;
    public readonly CharInfo MinBounds;
    private readonly uint _pad1;
    public readonly CharInfo MaxBounds;
    private readonly uint _pad2;
    public readonly ushort MinChar;
    public readonly ushort MaxChar;
    public readonly ushort DefaultChar;
    public readonly ushort PropertieLenght;
    public readonly FontDraw Direction;
    public readonly byte MinByte;
    public readonly byte MaxByte;
    private readonly byte _allCharsExist;
    public readonly ushort FontAscent;
    public readonly ushort FontDescent;
    public readonly uint InfoLenght;

    public bool Verify(in int sequence)
    {
        return _pad1 == 0 && _pad2 == 0 && this.Length > 7;
    }

    public readonly bool AllCharsExist => _allCharsExist == 1;
}