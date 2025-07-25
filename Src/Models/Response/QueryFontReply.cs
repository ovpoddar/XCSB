using System.Net.Sockets;
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
    public CharInfo[] CharInfo;
    public FontProp[] Properties;
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
        
        var requiredSize = (int)(result.InfoLenght * 4) + (result.FontLenght * 12);
        using var buffer = new ArrayPoolUsing<byte>(requiredSize);
        var receive = socket.Receive(buffer);
        Console.WriteLine(receive);
        throw new NotImplementedException();
    }
}