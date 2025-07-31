using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct QueryFontResponse : IXBaseResponse
{
    public readonly ResponseHeader ResponseHeader;
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

    public bool Verify()
    {
        return this.ResponseHeader.Verify() && this._pad1 == 0 && this._pad2 == 0 && this.ResponseHeader.Length > 7;
    }

    public readonly bool AllCharsExist => this._allCharsExist == 1;
}