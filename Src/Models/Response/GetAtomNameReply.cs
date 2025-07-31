using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

public readonly struct GetAtomNameReply 
{
    public readonly byte Reply;
    public readonly ushort Sequence;
    public readonly string Name;

    internal GetAtomNameReply(GetAtomNameResponse response, Socket socket)
    {
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        if (response.ResponseHeader.Length == 0)
            Name = string.Empty;
        else
        {
            using var nameBuffer = new ArrayPoolUsing<byte>((int)response.ResponseHeader.Length * 4);
            socket.ReceiveExact(nameBuffer);
            Name = Encoding.ASCII.GetString(nameBuffer, 0, response.LengthOfName);
        }
    }
}