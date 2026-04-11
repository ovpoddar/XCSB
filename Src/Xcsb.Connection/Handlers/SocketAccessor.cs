using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Xcsb.Connection.Configuration;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Models;
using Xcsb.Connection.Models.TypeInfo;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Connection.Handlers;

internal sealed class SocketAccessor : ISocketAccessor
{
    private readonly Socket _socket;
    public ISocketIn SocketIn { get; }
    public ISocketOut SocketOut { get; }
    
    public SocketAccessor(Socket socket, ConcurrentDictionary<(byte, byte?), MappingDetails> responseMap,
        XcsbClientConfiguration configuration)
    {
        _socket = socket;
        this.SocketIn = new SocketIn(socket, responseMap, configuration);
        this.SocketOut = new SocketOut(socket, configuration);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool PollRead(int timeout = -1) =>
        _socket.Poll(timeout, SelectMode.SelectRead);
    
    public int AvailableData => _socket.Available;
}