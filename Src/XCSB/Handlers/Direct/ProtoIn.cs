using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Configuration;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Response.Contract;
using Xcsb.Response.Errors;
using Xcsb.Response.Event;

namespace Xcsb.Handlers.Direct;

internal class ProtoIn : ProtoBase
{
    internal int Sequence { get; set; }
    internal ProtoIn(Socket socket, XcsbClientConfiguration configuration) : base(socket, configuration)
    {
        Sequence = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReceiveExact(scoped in Span<byte> buffer) =>
        Received(buffer);
}
