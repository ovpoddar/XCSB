using System.IO.MemoryMappedFiles;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

public readonly struct ListHostsReply
{
    public readonly byte Reply;
    public readonly AccessControlMode Mode;
    public readonly ushort Sequence;
    public readonly ushort NumberOfHosts;
    public readonly uint[] Hosts;

    internal ListHostsReply(ListHostsResponse response, Socket socket)
    {
        this.Reply = response.Reply;
        this.Mode = response.Mode;
        this.Sequence = response.Sequence;
        this.NumberOfHosts = response.NumberOfHosts;

        if (response.Length == 0)
            this.Hosts = [];
        else
        {
            var requiredSize = (int)response.Length * 4;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer[0..requiredSize]);
            this.Hosts = MemoryMarshal.Cast<byte, uint>(buffer[0..requiredSize]).ToArray();
        }
    }
}