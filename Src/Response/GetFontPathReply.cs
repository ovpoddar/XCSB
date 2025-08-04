using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using Xcsb.Helpers;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public struct GetFontPathReply
{
    public byte Reply;
    public ushort Sequence;
    public string[] Paths;

    internal GetFontPathReply(GetFontPathResponse response, Socket socket)
    {
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        if (response.StringLength == 0)
            Paths = [];
        else
        {
            var requiredSize = (int)response.ResponseHeader.Length * 4;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer[0..requiredSize]);
            this.Paths = new string[response.StringLength];
            int i = 0;
            foreach (var range in GenericHelper.GetNextStrValue(buffer))
                Paths[i++] = Encoding.UTF8.GetString(buffer, range.Position, range.Length);
        }
    }
}