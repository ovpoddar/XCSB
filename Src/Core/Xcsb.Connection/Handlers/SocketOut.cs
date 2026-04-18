using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Send<T>(scoped ref T value) where T : unmanaged =>
        this.SendRequest(
            MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref value, 1)),
            SocketFlags.None);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SendExact(scoped in ReadOnlySpan<byte> buffer) =>
        this.SendRequest(buffer, SocketFlags.None);
}