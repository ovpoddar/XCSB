using System.Net.Sockets;

namespace Xcsb.Connection.Handlers;

internal interface ISocketOut
{
    int Sequence { get; set; }
    
    void SendData(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags);
    void SendRequest(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags);
    void Send<T>(scoped ref T value) where T : unmanaged;
    void SendExact(scoped in ReadOnlySpan<byte> buffer);
}