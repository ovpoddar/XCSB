using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Handlers;

namespace Xcsb.Handlers.Direct;

internal sealed class ProtoOutExtended
{
    private readonly SoccketAccesser _soccketAccesser;

    public int Sequence
    {
        get { return _soccketAccesser.SendSequence; }
        set { _soccketAccesser.SendSequence = value; }
    }
    internal ProtoOutExtended(SoccketAccesser soccketAccesser)
    {
        _soccketAccesser = soccketAccesser;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Send<T>(scoped ref T value) where T : unmanaged =>
        _soccketAccesser.SendRequest(MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref value, 1)), SocketFlags.None);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SendExact(scoped in ReadOnlySpan<byte> buffer) =>
        _soccketAccesser.SendRequest(buffer, SocketFlags.None);

}
