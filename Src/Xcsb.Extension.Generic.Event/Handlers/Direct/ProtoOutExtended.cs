using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Handlers;

namespace Xcsb.Handlers.Direct;

internal sealed class ProtoOutExtended : ProtoBase
{

    public int Sequence
    {
        get { return base.SendSequence; }
        set { base.SendSequence = value; }
    }
    internal ProtoOutExtended(SoccketAccesser soccketAccesser) : base(soccketAccesser.Socket, soccketAccesser.Configuration)
    {
        Sequence = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Send<T>(scoped ref T value) where T : unmanaged =>
        this.SendExact(MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref value, 1)), SocketFlags.None);

    public override void SendExact(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags = SocketFlags.None)
    {
        base.SendExact(in buffer, socketFlags);
        Sequence++;
    }
}
