using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Xcsb.Configuration;

namespace Xcsb.Handlers;

internal sealed class SoccketAccesser : XcbSocketAccesser
{
    public SoccketAccesser(Socket socket, XcsbClientConfiguration configuration) : base(socket, configuration)
    { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void SendExact(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags = SocketFlags.None)
    {
        base.SendExact(buffer, socketFlags);
    }
}
