using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Configuration;

namespace Xcsb.Handlers.Direct;

internal class ProtoOut : ProtoBase
{
    internal int Sequence { get; set; }
    internal ProtoOut(Socket socket, XcsbClientConfiguration configuration) : base(socket, configuration)
    {
        Sequence = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SendExact(scoped in ReadOnlySpan<byte> buffer) =>
        this.SendExact(buffer, SocketFlags.None);

    protected override void SendExact(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
    {
        base.SendExact(in buffer, socketFlags);
        Sequence++;
    }
}
