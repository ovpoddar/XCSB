using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public struct QueryFontReply
{
    public byte Reply;
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

    internal QueryFontReply(QueryFontResponse result, Socket socket)
    {
        Reply = result.ResponseHeader.Reply;
        Sequence = result.ResponseHeader.Sequence;
        MinBounds = result.MinBounds;
        MaxBounds = result.MaxBounds;
        MinChar = result.MinChar;
        MaxChar = result.MaxChar;
        DefaultChar = result.DefaultChar;
        Direction = result.Direction;
        MinByte = result.MinByte;
        MaxByte = result.MaxByte;
        FontAscent = result.FontAscent;

        if (result.PropertieLenght == 0)
            Properties = [];
        else
        {
            var requireLength = result.PropertieLenght * 8;
            using var buffer = new ArrayPoolUsing<byte>(requireLength);
            socket.ReceiveExact(buffer[0..requireLength]);
            Properties = MemoryMarshal.Cast<byte, FontProp>(buffer[0..requireLength]).ToArray();
        }

        if (result.InfoLenght == 0)
            CharInfo = [];
        else
        {
            var requireLength = (int)result.InfoLenght * 12;
            using var buffer = new ArrayPoolUsing<byte>(requireLength);
            socket.ReceiveExact(buffer[0..requireLength]);
            CharInfo = MemoryMarshal.Cast<byte, CharInfo>(buffer[0..requireLength]).ToArray();
        }
    }
}