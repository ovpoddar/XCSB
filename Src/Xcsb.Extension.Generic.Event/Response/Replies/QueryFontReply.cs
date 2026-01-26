using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;
using Xcsb.Extension.Generic.Event.Response.Contract;
using Xcsb.Extension.Generic.Event.Response.Replies.Internals;
using Xcsb.Helpers;

namespace Xcsb.Extension.Generic.Event.Response.Replies;

public struct QueryFontReply
{
    public ResponseType Reply;
    public ushort Sequence;
    public CharInfo MinBounds;
    public CharInfo MaxBounds;
    public ushort MinChar;
    public ushort MaxChar;
    public ushort DefaultChar;
    public FontDraw Direction;
    public byte MinByte;
    public byte MaxByte;
    public ushort FontAscent;
    public ushort FontDescent;
    public FontProp[] Properties;
    public CharInfo[] CharInfo;

    internal QueryFontReply(Span<byte> response)
    {
        ref readonly var context = ref response.AsStruct<QueryFontResponse>();
        Reply = context.ResponseHeader.Reply;
        Sequence = context.ResponseHeader.Sequence;
        MinBounds = context.MinBounds;
        MaxBounds = context.MaxBounds;
        MinChar = context.MinChar;
        MaxChar = context.MaxChar;
        DefaultChar = context.DefaultChar;
        Direction = context.Direction;
        MinByte = context.MinByte;
        MaxByte = context.MaxByte;
        FontAscent = context.FontAscent;

        var cursor = Unsafe.SizeOf<QueryFontResponse>();
        if (context.PropertiesLength == 0)
            Properties = [];
        else
        {
            var length = context.PropertiesLength * 8;
            Properties = MemoryMarshal.Cast<byte, FontProp>(response.Slice(cursor, length)).ToArray();
            cursor += length;
        }

        if (context.InfoLenght == 0)
            CharInfo = [];
        else
        {
            var length = (int)context.InfoLenght * 12;
            CharInfo = MemoryMarshal.Cast<byte, CharInfo>(response.Slice(cursor, length)).ToArray();
        }
    }
}