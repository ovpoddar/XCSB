using System.Net.Sockets;
using System.Text;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

public struct GetFontPathReply
{
    public byte Reply;
    public ushort Sequence;
    public string Path;

    internal GetFontPathReply(GetFontPathResponse response, Socket socket)
    {
        this.Reply = response.Reply;
        this.Sequence = response.Sequence;
        if (response.StringLength == 0)
            this.Path = string.Empty;
        else
        {
            using var buffer = new ArrayPoolUsing<byte>((int)response.Length);
            socket.ReceiveExact(buffer[0..(int)response.Length]);
            this.Path = Encoding.UTF8.GetString(buffer, 0, response.StringLength);
        }
    }
}