using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xcsb.Helpers;
using Xcsb.Models.Event;

namespace Xcsb;
internal class BaseProtoClient
{
    internal readonly Socket socket;
    internal ushort sequenceNumber;
    internal readonly Stack<XEvent> bufferEvents;
    public BaseProtoClient(Socket socket)
    {
        this.socket = socket;
        bufferEvents = new Stack<XEvent>();
    }

    internal (T? result, ErrorEvent? error) ReceivedResponse<T>() where T : struct
    {
        if (socket.Available == 0)
            socket.Poll(-1, SelectMode.SelectRead);

        Span<byte> buffer = stackalloc byte[Marshal.SizeOf<XEvent>()];
        while (socket.Available != 0)
        {
            socket.ReceiveExact(buffer);
            ref var content = ref buffer.AsStruct<XEvent>();
            switch (content.EventType)
            {
                case EventType.Error:
                    return (null, content.ErrorEvent);
                case (EventType)1:
                    return (buffer.AsStruct<T>(), null);
                default:
                    bufferEvents.Push(content);
                    break;
            }
        }
        throw new Exception();
    }

    internal ErrorEvent? Received()
    {
        if (socket.Available == 0)
            return null;

        Span<byte> buffer = stackalloc byte[Marshal.SizeOf<XEvent>()];
        while (socket.Available != 0)
        {
            socket.ReceiveExact(buffer);
            ref var content = ref buffer.AsStruct<XEvent>();
            switch (content.EventType)
            {
                case EventType.Error:
                    return content.ErrorEvent;
                case (EventType)1:
                    break;
                default:
                    bufferEvents.Push(content);
                    break;
            }
        }
        return null;
    }
}
