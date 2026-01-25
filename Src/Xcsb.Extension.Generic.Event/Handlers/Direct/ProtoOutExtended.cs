using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Handlers.Direct;

namespace Xcsb.Extension.Generic.Event.Handlers.Direct;

internal sealed class ProtoOutExtended : ProtoOut
{

    internal ProtoOutExtended(ProtoOut protoOut) : base(protoOut.Socket, protoOut.Configuration)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Send<T>(scoped ref T value) where T : unmanaged =>
        this.SendExact(MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref value, 1)), SocketFlags.None);
}
