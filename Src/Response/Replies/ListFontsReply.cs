using System.Net.Sockets;
using System.Text;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public readonly struct ListFontsReply
{
    public readonly byte Reply;
    public readonly ushort Sequence;
    public readonly string[] Fonts;
    internal ListFontsReply(ListFontsResponse result, Socket socket)
    {
        Reply = result.ResponseHeader.Reply;
        Sequence = result.ResponseHeader.Sequence;
        if (result.NumberOfFonts == 0)
            Fonts = [];
        else
        {
            var requiredSize = (int)result.ResponseHeader.Length * 4;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer[0..requiredSize]);
            Fonts = new string[result.NumberOfFonts];
            var index = 0;
            foreach (var range in GenericHelper.GetNextStrValue(buffer))
                Fonts[index++] = Encoding.ASCII.GetString(buffer, range.Position, range.Length);
        }
    }
}