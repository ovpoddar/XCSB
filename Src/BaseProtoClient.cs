using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Event;
using Xcsb.Helpers;
using Xcsb.Response.Contract;
using Xcsb.Response.Errors;

namespace Xcsb;

internal class BaseProtoClient
{
    internal readonly Stack<XEvent> bufferEvents;
    internal readonly Socket socket;
    internal ushort sequenceNumber;

    public BaseProtoClient(Socket socket)
    {
        this.socket = socket;
        bufferEvents = new Stack<XEvent>();
    }

#if !NETSTANDARD
    [SkipLocalsInit]
#endif
    internal (T? result, XError? error) ReceivedResponseAndVerify<T>() where T : unmanaged, IXReply
    {
        sequenceNumber++;
        var resultLength = Marshal.SizeOf<T>();
        Span<byte> buffer = stackalloc byte[resultLength];
        while (true)
        {
            socket.EnsureReadSize(resultLength);
            socket.ReceiveExact(buffer[0..32]);
            ref var content = ref buffer[0..32].AsStruct<XResponse>();
            var responseType = content.GetResponseType();
            // this is the response verification. which ensures the reply type.
            Debug.Assert(content.Verify(sequenceNumber));

            switch (responseType)
            {
                case XResponseType.Invalid:
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
                    var error = content.As<XError>();
                    if(error.Verify(sequenceNumber))
                        return (null, error);
                    break;
                }
                case XResponseType.Event:
                case XResponseType.Notify:
                {
                    var eventContent = content.As<XEvent>();
                    if (eventContent.Verify(sequenceNumber))
                        bufferEvents.Push(content.As<XEvent>());
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    internal XError? Received()
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
                    return content.As<XError>();
                case XResponseType.Event:
                case XResponseType.Notify:
                    bufferEvents.Push(content.As<XEvent>());
                    break;
                case XResponseType.Invalid:
                    throw new ArgumentOutOfRangeException();
                case XResponseType.Reply:
                default:
                    continue;
            }
        }

        return null;
    }
}