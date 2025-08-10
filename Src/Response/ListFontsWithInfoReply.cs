using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public readonly struct ListFontsWithInfoReply
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
        AllCharsExist = result.AllCharsExist == 1;
        FontAscent = result.FontAscent;
        FontDescent = result.FontDescent;
        ReplyHint = result.ReplyHint;
        if (result.PropertiLenght == 0)
            Properties = [];
        else
        {
            var requiredSize = result.PropertiLenght * 8;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer[0..requiredSize]);
            Properties = MemoryMarshal.Cast<byte, ulong>(buffer).ToArray();
        }

        if (result.NameLength == 0)
            Name = string.Empty;
        else
        {
            var requiredSize = result.NameLength.AddPadding();
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer[0..requiredSize]);
            Name = Encoding.UTF8.GetString(buffer, 0, result.NameLength);
        }
    }
}