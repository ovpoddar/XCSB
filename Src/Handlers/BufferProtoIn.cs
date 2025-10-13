using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Xcsb.Helpers;
using Xcsb.Models.Infrastructure.Exceptions;
using Xcsb.Response.Contract;
using Xcsb.Response.Errors;
using Xcsb.Response.Event;

namespace Xcsb.Handlers;

internal class BufferProtoIn
{
    internal readonly ProtoIn ProtoIn;

    public BufferProtoIn(ProtoIn protoIn)
    {
        ProtoIn = protoIn;
    }

    internal void FlushSocket(in int requestLength, bool shouldThrowOnError)
    {
        if (ProtoIn.Socket.Available == 0)
            ProtoIn.Socket.Poll(1000, SelectMode.SelectRead);

        var bufferSize = Unsafe.SizeOf<XResponse>();
        Span<byte> buffer = stackalloc byte[bufferSize];
        while (ProtoIn.Socket.Available != 0)
        {
            ProtoIn.Socket.ReceiveExact(buffer);
            ref var content = ref buffer.AsStruct<XResponse>();
            var responseType = content.GetResponseType();
            switch (responseType)
            {
                case XResponseType.Event:
                case XResponseType.Notify:
                case XResponseType.Unknown:
                    ProtoIn.bufferEvents.Enqueue(content.As<GenericEvent>());
                    break;
                case XResponseType.Error:
                    if (shouldThrowOnError)
                        throw new XEventException(buffer.ToStruct<GenericError>());
                    break;
                case XResponseType.Reply:
                    ComputeResponse(ref buffer, ProtoIn.Socket);
                    break;
                default:
                    throw new  Exception(string.Join(", ", buffer.ToArray()));
            }
        }

        ProtoIn.Sequence += requestLength;
    }


    private void ComputeResponse(ref Span<byte> buffer, Socket socket)
    {
        ref var content = ref buffer.AsStruct<RepliesHeader>();
        var replySize = (int)(content.Length * 4);
        Span<byte> result = stackalloc byte[32 + replySize];
        buffer.CopyTo(result[..32]);

        socket.EnsureReadSize((int)replySize);
        socket.ReceiveExact(result[32..]);

        if (ProtoIn.replyBuffer.ContainsKey(content.Sequence))
        {
            var response = ProtoIn.replyBuffer[content.Sequence];
            replySize = result.Length + response.Length;
            using var scratchBuffer = new ArrayPoolUsing<byte>(replySize);
            response.CopyTo(scratchBuffer);
            result.CopyTo(scratchBuffer[response.Length..]);
            ProtoIn.replyBuffer[content.Sequence] = scratchBuffer[..replySize].ToArray();
        }
        else
        {
            ProtoIn.replyBuffer[content.Sequence] = result.ToArray();
        }
    }
}