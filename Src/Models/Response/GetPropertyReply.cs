using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

public readonly struct GetPropertyReply
{
    public readonly byte Reply;
    public readonly byte Format;
    public readonly ushort Sequence;
    public readonly uint Type;
    public readonly byte[] Data;

    internal GetPropertyReply(GetPropertyResponse response, Socket socket)
    {
        this.Reply = response.Reply;
        this.Format = response.Format;
        this.Sequence = response.Sequence;
        this.Type = response.Type;
        
        if (response.Length == 0)
            Data = [];
        else
        {
            var data = new byte[response.Length * 4];
            socket.ReceiveExact(data);
            Data = data;
        }
    }


}