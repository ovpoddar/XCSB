using System.Runtime.InteropServices;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct QueryFontResponse : IXBaseResponse
{
    public readonly byte Reply;
    private readonly byte _pad0;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly CharInfo MinBounds;
    private readonly uint _pad1;
    public readonly CharInfo MaxBounds;
    private readonly uint _pad2;
    public readonly ushort MinChar;
    public readonly ushort MaxChar;
    public readonly ushort DefaultChar;
    public readonly ushort FontLenght;
    public readonly FontDraw Direction;
    public readonly byte MinByte;
    public readonly byte MaxByte;
    private readonly byte _allCharsExist;
    public readonly ushort FontAscent;
    public readonly ushort FontDescent;
    public readonly uint InfoLenght;

    public bool Verify()
    {
        return this.Reply == 1 && this._pad0 == 0 && this._pad1 == 0 && this._pad2 == 0 && this.Length > 7;
    }

    public readonly bool AllCharsExist => this._allCharsExist == 1;
}