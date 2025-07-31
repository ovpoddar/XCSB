using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

public readonly struct QueryTreeReply
{
    public readonly byte Reply;
    public readonly ushort Sequence;
    public readonly uint Root;
    public readonly uint Parent;
    public readonly uint[] WindowChildren;

    internal QueryTreeReply(QueryTreeResponse response, Socket socket)
    {
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        Root = response.Root;
        Parent = response.Parent;
        
        if (response.WindowChildrenLenght == 0)
            WindowChildren = [];
        else
        {
            using var windowChildren = new ArrayPoolUsing<byte>(response.WindowChildrenLenght * 4);
            socket.ReceiveExact(windowChildren);
            WindowChildren = MemoryMarshal.Cast<byte, uint>(windowChildren).ToArray();
        }
    }
}