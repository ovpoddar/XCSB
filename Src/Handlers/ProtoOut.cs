using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Src.Handlers;

internal struct ProtoOut
{
    private readonly Socket _socket;

    internal int Sequence { get; set; }
    internal ProtoOut(Socket socket)
    {
        this._socket = socket;
        this.Sequence = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Send<T>(scoped ref T value) where T : unmanaged =>
        SendExact(MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref value, 1)), SocketFlags.None);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SendExact(scoped in ReadOnlySpan<byte> buffer) =>
        SendExact(buffer, SocketFlags.None);

    private void SendExact(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
    {
        var total = 0;
        while (total < buffer.Length)
        {
            var sent = _socket.Send(buffer[total..], socketFlags);
            if (sent <= 0)
                throw new SocketException();
            total += sent;
        }
        this.Sequence++;
    }
}
