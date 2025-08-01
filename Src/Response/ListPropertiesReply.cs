using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public readonly struct ListPropertiesReply
{
    public readonly byte Reply;
    public readonly ushort Sequence;
    public readonly uint[] Atoms;

    internal ListPropertiesReply(ListPropertiesResponse response, Socket socket)
    {
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;

        if (response.NumberOfProperties == 0)
            Atoms = [];
        else
        {
            var atoms = new ArrayPoolUsing<byte>(response.NumberOfProperties * 4);
            socket.ReceiveExact(atoms);
            Atoms = MemoryMarshal.Cast<byte, uint>(atoms).ToArray();
        }
    }
}