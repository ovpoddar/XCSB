using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

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
        this.Reply = result.Reply;
        this.Sequence = result.Sequence;
        this.MinBounds = result.MinBounds;
        this.MaxBounds = result.MaxBounds;
        this.MinChar = result.MinChar;
        this.MaxChar = result.MaxChar;
        this.DefaultChar = result.DefaultChar;
        this.Direction = result.Direction;
        this.MinByte = result.MinByte;
        this.MaxByte = result.MaxByte;
        this.FontAscent = result.FontAscent;

        if (result.PropertieLenght == 0)
            this.Properties = [];
        else
        {
            var requireLength = result.PropertieLenght * 8;
            using var buffer = new ArrayPoolUsing<byte>(requireLength);
            socket.ReceiveExact(buffer[0..requireLength]);
            this.Properties = MemoryMarshal.Cast<byte, FontProp>(buffer[0..requireLength]).ToArray();
        }

        if (result.InfoLenght == 0)
            this.CharInfo = [];
        else
        {
            var requireLength = (int)result.InfoLenght * 12;
            using var buffer = new ArrayPoolUsing<byte>(requireLength);
            socket.ReceiveExact(buffer[0..requireLength]);
            this.CharInfo = MemoryMarshal.Cast<byte, CharInfo>(buffer[0..requireLength]).ToArray();
        }
    }
}