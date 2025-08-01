using System.Net.Sockets;
using System.Text;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public readonly struct ListExtensionsReply
{
    public readonly byte Reply;
    public readonly ushort Sequence;
    public readonly string[] Names;

    internal ListExtensionsReply(ListExtensionsResponse result, Socket socket)
    {
        Reply = result.Reply;
        Sequence = result.Sequence;
        if (result.NumberOfExtensions == 0)
            Names = [];
        else
        {
            var requiredSize = (int)result.Length * 4;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer[0..requiredSize]);
            Names = new string[result.NumberOfExtensions];
            var index = 0;
            foreach (var range in GenericHelper.GetNextStrValue(buffer))
                Names[index++] = Encoding.ASCII.GetString(buffer, range.Position, range.Length);
        }
    }
}