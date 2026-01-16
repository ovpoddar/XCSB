using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Handlers;

internal class ProtoOut : ProtoBase
{
    internal int Sequence { get; set; }
    internal ProtoOut(Socket socket, XcbClientConfiguration configuration) : base(socket, configuration)
    {
        Sequence = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Send<T>(scoped ref T value) where T : unmanaged =>
        this.SendExact(MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref value, 1)), SocketFlags.None);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SendExact(scoped in ReadOnlySpan<byte> buffer) =>
        this.SendExact(buffer, SocketFlags.None);

    protected override void SendExact(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
    {
        base.SendExact(in buffer, socketFlags);
        Sequence++;
    }
}
