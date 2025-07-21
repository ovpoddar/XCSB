using System.Net.Sockets;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

public readonly struct ListHostsReply
{
    public readonly byte Reply;
    public readonly AccessControlMode Mode;
    public readonly ushort Sequence;
    public readonly uint[] Hosts;

    internal ListHostsReply(ListHostsResponse response, Socket socket)
    {
        Reply = response.Reply;
        Mode = response.Mode;
        Sequence = response.Sequence;

        if (response.NumberOfHosts == 0)
            Hosts = [];
        else
        {
            throw new NotImplementedException();
        }
    }
}