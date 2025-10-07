using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Src.Response.Contract;
using Xcsb.Event;
using Xcsb.Helpers;
using Xcsb.Response.Contract;

namespace Src.Handlers;

internal struct ProtoIn
{
    private readonly Socket _socket;

    internal readonly Queue<GenericEvent> bufferEvents;
    internal List<(int, byte[])> replyBuffer;
    internal int Sequence { get; set; }
    internal ProtoIn(Socket socket)
    {
        this._socket = socket;
        this.Sequence = 0;
        replyBuffer = new List<(int, byte[])>();
    }

    public byte[] ReceivedResponse(int sequence)
    {
        //todo: timeout after some time.
        while (true)
        {
            if (sequence > this.Sequence)
            {
                FlushSocket();
            }
            else
            {
                var reply = replyBuffer.FirstOrDefault(a => a.Item1 == sequence);
                return replyBuffer.Remove(reply)
                    ? reply.Item2
                    : [];
            }
        }
    }

    private void FlushSocket()
    {
        var bufferSize = Unsafe.SizeOf<XResponse>();
        Span<byte> buffer = stackalloc byte[bufferSize];
        while (_socket.Available != 0)
        {
            _socket.EnsureReadSize(bufferSize);
            _socket.ReceiveExact(buffer);
            ref var content = ref buffer.AsStruct<XResponse>();
            var responseType = content.GetResponseType();
            switch (responseType)
            {
                case XResponseType.Event:
                case XResponseType.Notify:
                case XResponseType.Unknown:
                    bufferEvents.Enqueue(content.As<GenericEvent>());
                    break;
                case XResponseType.Error:
                    replyBuffer.Add((content.Sequence, buffer.ToArray()));
                    break;
                case XResponseType.Reply:
                    replyBuffer.Add((content.Sequence, ComputeResponse(ref buffer, _socket)));
                    break;
                default:
                    throw new Exception(string.Join(", ", buffer.ToArray()));
            }
        }
    }

    private byte[] ComputeResponse(ref Span<byte> buffer, Socket socket)
    {
        ref var content = ref buffer.AsStruct<RepliesHeader>();
        if (content.Sequence > this.Sequence)
            this.Sequence = content.Sequence;

        var replySize = content.Length * 4;
        Span<byte> result = stackalloc byte[(int)(32 + replySize)];
        buffer.CopyTo(result[..32]);
        socket.EnsureReadSize((int)replySize);
        socket.ReceiveExact(result[32..]);
        return result.ToArray();
    }

}
