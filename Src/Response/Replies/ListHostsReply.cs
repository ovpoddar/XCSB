using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Response.Contract;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public readonly struct ListHostsReply
{
    public readonly ResponseType Reply;
    public readonly AccessControlMode Mode;
    public readonly ushort Sequence;
    public readonly ushort NumberOfHosts;
    public readonly uint[] Hosts;

    internal ListHostsReply(ListHostsResponse response, Socket socket)
    {
        Reply = response.ResponseHeader.Reply;
        Mode = response.ResponseHeader.GetValue();
        Sequence = response.ResponseHeader.Sequence;
        NumberOfHosts = response.NumberOfHosts;

        if (response.Length == 0)
            Hosts = [];
        else
        {
            var requiredSize = (int)response.Length * 4;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer[0..requiredSize]);
            Hosts = MemoryMarshal.Cast<byte, uint>(buffer[0..requiredSize]).ToArray();
        }
    }
}