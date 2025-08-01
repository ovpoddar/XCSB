using System.Net.Sockets;
using System.Text;
using Xcsb.Helpers;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public struct GetFontPathReply
{
    public byte Reply;
    public ushort Sequence;
    public string Path;

    internal GetFontPathReply(GetFontPathResponse response, Socket socket)
    {
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        if (response.StringLength == 0)
            Path = string.Empty;
        else
        {
            using var buffer = new ArrayPoolUsing<byte>((int)response.ResponseHeader.Length);
            socket.ReceiveExact(buffer[0..(int)response.ResponseHeader.Length]);
            Path = Encoding.UTF8.GetString(buffer, 0, response.StringLength);
        }
    }
}