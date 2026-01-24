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


    internal void FlushSocket()
    {
        var bufferSize = Unsafe.SizeOf<XResponse>();
        Span<byte> buffer = stackalloc byte[bufferSize];
        while (Socket.Available != 0)
        {
            _ = Received(buffer);
            ref readonly var content = ref buffer.AsStruct<XResponse>();
            var responseType = content.GetResponseType();
            switch (responseType)
            {
                case XResponseType.Event:
                case XResponseType.Notify:
                case XResponseType.Unknown:
                    BufferEvents.Enqueue(content.As<GenericEvent>());
                    break;
                case XResponseType.Error:
                    ReplyBuffer[content.Sequence] = buffer.ToArray();
                    break;
                case XResponseType.Reply:
                    ReplyBuffer[content.Sequence] = ComputeResponse(ref buffer);
                    break;
                default:
                    throw new Exception(string.Join(", ", buffer.ToArray()));
            }
        }
    }

    internal byte[] ComputeResponse(ref Span<byte> buffer)
    {
        ref readonly var content = ref buffer.AsStruct<RepliesHeader>();
        if (content.Sequence > Sequence)
            Sequence = content.Sequence;

        var replySize = (int)(content.Length * 4);
        if (replySize == 0)
            return buffer.ToArray();

        using var result = new ArrayPoolUsing<byte>(32 + replySize);
        buffer.CopyTo(result[..32]);

        _ = Received(result[32..result.Length]);

        if (!ReplyBuffer.TryRemove(content.Sequence, out var response))
            return result;

        replySize = result.Length + response.Length;
        using var scratchBuffer = new ArrayPoolUsing<byte>(replySize);
        response.CopyTo(scratchBuffer);
        result[0..result.Length].CopyTo(scratchBuffer[response.Length..]);
        return scratchBuffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReceiveExact(scoped in Span<byte> buffer) =>
        Received(buffer);
}
