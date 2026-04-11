using System.Collections.Concurrent;
using System.Net.Sockets;
using Xcsb.Connection.Configuration;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Models;

namespace Xcsb.Connection.Handlers;

internal class SocketOut : ISocketOut
{
    private readonly Socket _socket;
    private readonly XcsbClientConfiguration _configuration;

    public SocketOut(Socket socket, XcsbClientConfiguration configuration)
    {
        _socket = socket;
        _configuration = configuration;
    }
    
    public int Sequence { get; set; }

    public void SendData(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
    {
        _socket.SendExact(buffer, socketFlags);
        _configuration.OnSendRequest?.Invoke(buffer);
    }

    public void SendRequest(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
    {
        SendData(in buffer, socketFlags);
        Sequence++;
    }
}