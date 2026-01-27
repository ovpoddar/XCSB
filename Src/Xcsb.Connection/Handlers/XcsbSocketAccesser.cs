using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Xcsb.Connection.Configuration;
using Xcsb.Connection.Helpers;

namespace Xcsb.Connection.Handlers;

internal abstract class XcsbSocketAccesser
{
    internal readonly XcsbClientConfiguration Configuration;
    internal readonly Socket Socket;
    internal int ReceivedSequence = 0;
    internal int SendSequence = 0;

    protected XcsbSocketAccesser(Socket socket, XcsbClientConfiguration configuration)
    {
        Socket = socket;
        Configuration = configuration;
    }

    public virtual void SendExact(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
    {
        Socket.SendExact(buffer, socketFlags);
        Configuration.OnSendRequest?.Invoke(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Received(scoped in Span<byte> buffer, bool readAll = true)
    {
        if (readAll)
        {
            Socket.ReceiveExact(buffer);
            Configuration.OnReceivedReply?.Invoke(buffer);
            return buffer.Length;
        }
        else
        {
            var totalRead = Socket.Receive(buffer);
            Configuration.OnReceivedReply?.Invoke(buffer);
            return totalRead;
        }
    }

}