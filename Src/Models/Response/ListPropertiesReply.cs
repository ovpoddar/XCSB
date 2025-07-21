using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

public struct ListPropertiesReply
{
    public readonly byte Reply;
    public readonly ushort Sequence;
    public uint[] Atoms;

    internal ListPropertiesReply(ListPropertiesResponse response, Socket socket)
    {
        Reply = response.Reply;
        Sequence = response.Sequence;

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