using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Xcsb.Connection.Helpers;
using Xcsb.Handlers.Direct;
using Xcsb.Infrastructure.Exceptions;
using Xcsb.Response.Contract;
using Xcsb.Response.Errors;
using Xcsb.Response.Event;

namespace Xcsb.Handlers.Buffered;

internal sealed class BufferProtoIn
{
    internal readonly ProtoInExtended ProtoIn;

    public BufferProtoIn(ProtoInExtended protoIn)
    {
        ProtoIn = protoIn;
    }

    internal void FlushSocket(in int requestLength, int outProtoSequence, bool shouldThrowOnError)
    {
        if (ProtoIn._soccketAccesser.Socket.Available == 0)
            ProtoIn._soccketAccesser.Socket.Poll(1000, SelectMode.SelectRead);

        var bufferSize = Unsafe.SizeOf<XResponse>();
        Span<byte> buffer = stackalloc byte[bufferSize];
        while (ProtoIn._soccketAccesser.Socket.Available != 0)
        {
            _ = ProtoIn._soccketAccesser.Received(buffer);
            ref readonly var content = ref buffer.AsStruct<XResponse>();
            var responseType = content.GetResponseType();
            switch (responseType)
            {
                case XResponseType.Event:
                case XResponseType.Notify:
                case XResponseType.Unknown:
                    ProtoIn._soccketAccesser.BufferEvents.Enqueue(buffer.ToArray());
                    break;
                case XResponseType.Error:
                    if (ProtoIn.Sequence > outProtoSequence)
                        ProtoIn._soccketAccesser.ReplyBuffer[content.Sequence] = buffer.ToArray();
                    else
                    {
                        if (shouldThrowOnError)
                            throw new XEventException(buffer.ToStruct<GenericError>());
                    }
                    break;
                case XResponseType.Reply:
                    ProtoIn._soccketAccesser.ReplyBuffer[content.Sequence] = ComputeResponse(ref buffer);
                    break;
                default:
                    throw new Exception(string.Join(", ", buffer.ToArray()));
            }
        }

        ProtoIn.Sequence += requestLength;
    }

    private byte[] ComputeResponse(ref Span<byte> buffer)
    {
        ref readonly var content = ref buffer.AsStruct<RepliesHeader>();

        var replySize = (int)(content.Length * 4);
        if (replySize == 0)
            return buffer.ToArray();

        using var result = new ArrayPoolUsing<byte>(32 + replySize);
        buffer.CopyTo(result[..32]);

        _ = ProtoIn._soccketAccesser.Received(result[32..result.Length]);


        if (!ProtoIn._soccketAccesser.ReplyBuffer.TryRemove(content.Sequence, out var response))
            return result;

        replySize = result.Length + response.Length;
        using var scratchBuffer = new ArrayPoolUsing<byte>(replySize);
        response.CopyTo(scratchBuffer);
        result[0..result.Length].CopyTo(scratchBuffer[response.Length..]);
        return scratchBuffer;
    }


}