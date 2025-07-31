using System.Net.Sockets;
using System.Text;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

public struct ListFontsReply
{
    public readonly byte Reply;
    public readonly ushort Sequence;
    public readonly string[] Fonts;
    internal ListFontsReply(ListFontsResponse result, Socket socket)
    {
        this.Reply = result.ResponseHeader.Reply;
        this.Sequence = result.ResponseHeader.Sequence;
        if (result.NumberOfFonts == 0)
            this.Fonts = [];
        else
        {
            var requiredSize = (int)result.ResponseHeader.Length * 4;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer[0..requiredSize]);
            this.Fonts = new string[result.NumberOfFonts];
            var index = 0;
            foreach (var range in GetNextStrValue(buffer))
                this.Fonts[index++] = Encoding.ASCII.GetString(buffer, range.Position, range.Length);
        }
    }
    // todo move to a shared space
    private IEnumerable<DataRange> GetNextStrValue(ArraySegment<byte> buffer)
    {
        var index = 0;
        while (index < buffer.Count)
        {
            var length = buffer[index++];
            if (length == 0)
                break;
            if (index + length > buffer.Count)
                yield return new DataRange(index, buffer.Count - index);
            else
                yield return new DataRange(index, length);
            index += length;
        }
    }
}