using System.Net.Sockets;
using System.Text;
using Xcsb.Helpers;
using Xcsb.Response.Contract;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public readonly struct GetAtomNameReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly string Name;

    internal GetAtomNameReply(GetAtomNameResponse response, Socket socket)
    {
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        if (response.Length == 0)
            Name = string.Empty;
        else
        {
            using var nameBuffer = new ArrayPoolUsing<byte>((int)response.Length * 4);
            socket.ReceiveExact(nameBuffer);
            Name = Encoding.ASCII.GetString(nameBuffer, 0, response.LengthOfName);
        }
    }
}