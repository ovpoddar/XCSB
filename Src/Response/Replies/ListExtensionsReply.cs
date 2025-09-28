using System.Net.Sockets;
using System.Text;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Response.Contract;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public readonly struct ListExtensionsReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly string[] Names;

    internal ListExtensionsReply(ListExtensionsResponse result, Socket socket)
    {
        Reply = result.ResponseHeader.Reply;
        Sequence = result.ResponseHeader.Sequence;
        if (result.ResponseHeader.GetValue() == 0)
            Names = [];
        else
        {
            var requiredSize = (int)result.Length * 4;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer[0..requiredSize]);
            Names = new string[result.ResponseHeader.GetValue()];
            var index = 0;
            foreach (var range in GenericHelper.GetNextStrValue(buffer))
                Names[index++] = Encoding.ASCII.GetString(buffer, range.Position, range.Length);
        }
    }
}