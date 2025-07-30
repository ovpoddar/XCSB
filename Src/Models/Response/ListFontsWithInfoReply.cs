using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

public struct ListFontsWithInfoReply
{
    public readonly byte Reply;
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
    public readonly ulong[] Properties;
    public readonly string Name;

    internal ListFontsWithInfoReply(ListFontsWithInfoResponse result, Socket socket)
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
        this.AllCharsExist = result.AllCharsExist == 1;
        this.FontAscent = result.FontAscent;
        this.FontDescent = result.FontDescent;
        this.ReplyHint = result.ReplyHint;
        if (result.PropertiLenght == 0)
            this.Properties = [];
        else
        {
            var requiredSize = (int)result.PropertiLenght * 8;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer[0..requiredSize]);
            this.Properties = MemoryMarshal.Cast<byte, ulong>(buffer).ToArray();
        }
        
        if (result.NameLength == 0)
            this.Name = string.Empty;
        else
        {
            var requiredSize = (int)result.NameLength.AddPadding() * 4;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer[0..requiredSize]);
            this.Name = Encoding.UTF8.GetString(buffer, 0, result.NameLength);
        }
    }
}