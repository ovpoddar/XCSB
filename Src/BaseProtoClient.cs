using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Event;
using Xcsb.Helpers;
using Xcsb.Models.Infrastructure.Exceptions;
using Xcsb.Response.Contract;
using Xcsb.Response.Errors;

namespace Xcsb;

internal class BaseProtoClient
{
    internal readonly Stack<GenericEvent> bufferEvents;
    internal readonly Socket socket;
    internal ushort sequenceNumber;

    public BaseProtoClient(Socket socket)
    {
        this.socket = socket;
        bufferEvents = new Stack<GenericEvent>();
    }

#if !NETSTANDARD
    [SkipLocalsInit]
#endif
    internal (T? result, GenericError? error) ReceivedResponseAndVerify<T>(bool exhaustSocket = false)
        where T : unmanaged, IXReply
    {
        sequenceNumber++;
        var resultLength = Marshal.SizeOf<T>();
        Span<byte> buffer = stackalloc byte[resultLength];
        if (!exhaustSocket)
        {
            while (true)
            {
                socket.EnsureReadSize(32);
                socket.ReceiveExact(buffer[0..32]);
                ref var content = ref buffer[0..32].AsStruct<XResponse>();
                var responseType = content.GetResponseType();
                // this is the response verification. which ensures the reply type.
                Debug.Assert(content.Verify(sequenceNumber));

                switch (responseType)
                {
                    case XResponseType.Unknown:
                        throw new Exception("Should not come here");
                    case XResponseType.Reply:
                        {
                            socket.ReceiveExact(buffer[32..]);
                            ref var responce = ref buffer.AsStruct<T>();
                            // this is the reply verification. which ensures the structure of the reply.
                            if (responce.Verify(sequenceNumber))
                                return (buffer.ToStruct<T>(), null);
                            break;
                        }
                    case XResponseType.Error:
                        {
                            var error = content.As<GenericError>();
                            if (error.Verify(sequenceNumber))
                                return (null, error);
                            break;
                        }
                    case XResponseType.Event:
                    case XResponseType.Notify:
                        {
                            var eventContent = content.As<GenericEvent>();
                            if (eventContent.Verify(sequenceNumber))
                                bufferEvents.Push(content.As<GenericEvent>());
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        else
        {
            var result = new byte[32];
            var isError = false;
            while (socket.Available != 0)
            {
                socket.EnsureReadSize(32);
                socket.ReceiveExact(buffer[0..32]);
                ref var content = ref buffer[0..32].AsStruct<XResponse>();
                Debug.Assert(content.Verify(sequenceNumber));

                var responseType = content.GetResponseType();
                switch (responseType)
                {
                    case XResponseType.Reply:
                        {
                            socket.ReceiveExact(buffer[32..]);
                            ref var responce = ref buffer.AsStruct<T>();
                            // this is the reply verification. which ensures the structure of the reply.
                            if (responce.Verify(sequenceNumber))
                            {
                                result = buffer.ToArray();
                                isError = false;
                            }
                            break;
                        }
                    case XResponseType.Error:
                        {
                            var error = content.As<GenericError>();
                            if (error.Verify(sequenceNumber))
                            {
                                result = buffer.ToArray();
                                isError = true;
                            }
                            break;
                        }
                    case XResponseType.Event:
                    case XResponseType.Notify:
                    case XResponseType.Unknown:
                        {
                            var eventContent = content.As<GenericEvent>();
                            if (eventContent.Verify(sequenceNumber))
                                bufferEvents.Push(content.As<GenericEvent>());
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return isError
                ? (null, result.AsSpan().ToStruct<GenericError>())
                : (result.AsSpan().ToStruct<T>(), null);
        }
    }

    internal GenericError? Received()
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
                    bufferEvents.Push(content.As<GenericEvent>());
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
