using System.Collections;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Src.Response.Contract;
using Xcsb.Event;
using Xcsb.Helpers;
using Xcsb.Models.Infrastructure.Exceptions;
using Xcsb.Response;
using Xcsb.Response.Contract;
using Xcsb.Response.Errors;
using Xcsb.Response.Internals;

namespace Xcsb;

internal class BaseProtoClient
{
    internal readonly Queue<GenericEvent> bufferEvents;
    internal readonly Socket socket;
    internal ushort sequenceNumber;

    public BaseProtoClient(Socket socket)
    {
        this.socket = socket;
        bufferEvents = new Queue<GenericEvent>();
    }

    private (T? result, GenericError? error, int? eventSequence) ParseResponse<T>(scoped ref Span<byte> receivedBuffer)
        where T : unmanaged, IXReply
    {
        ref var content = ref receivedBuffer.AsStruct<XResponse>();
        var responseType = content.GetResponseType();
        if (content.Verify(sequenceNumber))
        {
            if (responseType is XResponseType.Error)
                return (null, content.As<GenericError>(), null);

            socket.ReceiveExact(receivedBuffer[32..]);
            return (receivedBuffer.ToStruct<T>(), null, null);
        }

        var eventContent = content.As<GenericEvent>();
        if (eventContent.Verify(sequenceNumber))
            bufferEvents.Enqueue(content.As<GenericEvent>());
        return (null, null, eventContent.Sequence);
    }

#if !NETSTANDARD
    [SkipLocalsInit]
#endif
    internal (T? result, GenericError? error) ReceivedResponseAndVerify<T>(bool exhaustSocket = false)
        where T : unmanaged, IXReply
    {
        sequenceNumber++;
        var requestLength = Unsafe.SizeOf<T>();
        Span<byte> responseBuffer = stackalloc byte[requestLength];
        var breakCounter = 3;
        while (true)
        {
            socket.EnsureReadSize(32);
            socket.ReceiveExact(responseBuffer[..32]);
            var result = ParseResponse<T>(ref responseBuffer);
            if (result.eventSequence.HasValue)
            {
                if (result.eventSequence.Value > sequenceNumber || breakCounter == 0)
                    return (null, null);
                if (socket.Available == 0)
                    breakCounter--;

                continue;
            }

            if (exhaustSocket)
                result.result = ExhustSocket<T>() ?? result.result;
            return (result.result, result.error);
        }
    }

    private T? ExhustSocket<T>() where T : unmanaged, IXReply
    {
        if (socket.Available == 0)
            return null;
        Span<byte> buffer = stackalloc byte[32];
        T? result = default;
        while (socket.Available != 0 && socket.Available % 32 == 0)
        {
            socket.ReceiveExact(buffer);
            ref var content = ref buffer.AsStruct<XResponse>();
            var responseType = content.GetResponseType();
            if (responseType is not XResponseType.Event or XResponseType.Notify or XResponseType.Unknown)
            {
                result = buffer.ToStruct<T>();
                continue;
            }

            var eventContent = content.As<GenericEvent>();
            if (eventContent.Verify(sequenceNumber))
                bufferEvents.Enqueue(content.As<GenericEvent>());
        }

        return result;
    }

    public GenericError? Received()
    {
        if (socket.Available == 0)
            return null;

        Span<byte> buffer = stackalloc byte[Marshal.SizeOf<XResponse>()];
        while (socket.Available != 0)
        {
            socket.ReceiveExact(buffer);
            ref var content = ref buffer.AsStruct<XResponse>();
            var responseType = content.GetResponseType();
            switch (responseType)
            {
                case XResponseType.Error:
                    return content.As<GenericError>();
                case XResponseType.Event:
                case XResponseType.Notify:
                case XResponseType.Unknown:
                    bufferEvents.Enqueue(content.As<GenericEvent>());
                    break;
                case XResponseType.Reply:
                default:
                    continue;
            }
        }

        return null;
    }

    internal void ProcessEvents(bool throwOnError)
    {
        while (true)
        {
            var error = Received();
            if (error.HasValue && throwOnError)
                throw new XEventException(error.Value);
            if (socket.Available == 0)
                break;
        }
    }

}