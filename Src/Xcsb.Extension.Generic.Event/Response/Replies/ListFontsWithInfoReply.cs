using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Extension.Generic.Event.Response.Replies.Internals;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Replies;

public readonly struct ListFontsWithInfoReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly CharInfo MinBounds;
    public readonly CharInfo MaxBounds;
    public readonly ushort MinChar;
    public readonly ushort MaxChar;
    public readonly ushort DefaultChar;
    public readonly FontDraw Direction;
    public readonly byte MinByte;
    public readonly byte MaxByte;
    public readonly bool AllCharsExist;
    public readonly ushort FontAscent;
    public readonly ushort FontDescent;
    public readonly uint ReplyHint;
    public readonly FontProp[] Properties;
    public readonly string Name;

    internal ListFontsWithInfoReply(ref readonly ListFontsWithInfoResponse response, Span<byte> buffer)
    {

        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        MinBounds = response.MinBounds;
        MaxBounds = response.MaxBounds;
        MinChar = response.MinChar;
        MaxChar = response.MaxChar;
        DefaultChar = response.DefaultChar;
        Direction = response.Direction;
        MinByte = response.MinByte;
        MaxByte = response.MaxByte;
        AllCharsExist = response.AllCharsExist == 1;
        FontAscent = response.FontAscent;
        FontDescent = response.FontDescent;
        ReplyHint = response.ReplyHint;
        var cursor = 0;
        if (response.PropertiLenght == 0)
            Properties = [];
        else
        {
            var requiredSize = response.PropertiLenght * 8;
            Properties = MemoryMarshal.Cast<byte, FontProp>(buffer[cursor..requiredSize]).ToArray();
            cursor += requiredSize;
        }

        if (response.NameLength == 0)
            Name = string.Empty;
        else
        {
            Name = Encoding.UTF8.GetString(buffer.Slice(cursor, response.NameLength));
        }
    }
}