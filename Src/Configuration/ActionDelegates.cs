using System.Net.Sockets;

namespace Xcsb.Configuration;

public class ActionDelegates
{
    public delegate void SendAction(Socket socket, SocketFlags socketFlags, scoped in ReadOnlySpan<byte> request);

    public delegate void ReceivedAction(scoped in ReadOnlySpan<byte> response);
}