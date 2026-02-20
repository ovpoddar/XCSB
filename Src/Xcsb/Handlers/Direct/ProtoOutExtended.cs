using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Handlers;

namespace Xcsb.Handlers.Direct;

internal sealed class ProtoOutExtended
{
    private readonly ISocketAccessor _socketAccessor;

    public int Sequence
    {
        get { return _socketAccessor.SendSequence; }
        set { _socketAccessor.SendSequence = value; }
    }
    internal ProtoOutExtended(ISocketAccessor socketAccessor)
    {
        _socketAccessor = socketAccessor;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Send<T>(scoped ref T value) where T : unmanaged =>
        _socketAccessor.SendRequest(MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref value, 1)), SocketFlags.None);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SendExact(scoped in ReadOnlySpan<byte> buffer) =>
        _socketAccessor.SendRequest(buffer, SocketFlags.None);

}
