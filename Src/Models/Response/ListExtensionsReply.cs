using System.Net.Sockets;
using System.Text;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

public struct ListExtensionsReply
{
    public readonly byte Reply;
    public readonly ushort Sequence;
    public readonly string Names;

    internal ListExtensionsReply(ListExtensionsResponse result, Socket socket)
    {
        this.Reply = result.Reply;
        this.Sequence = result.Sequence;
        if (result.Length == 0)
            this.Names = string.Empty;
        else
        {
            var requiredSize = (int)result.Length * 4;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer[0..requiredSize]);
            this.Names = Encoding.UTF8.GetString(buffer, 0, result.NumberOfExtensions);
        }
    }
}