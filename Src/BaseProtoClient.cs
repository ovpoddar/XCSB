using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Event;
using Xcsb.Helpers;
using Xcsb.Response.Contract;

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
    internal (T? result, ErrorEvent? error) ReceivedResponse<T>() where T : unmanaged, IXBaseResponse
    {
        sequenceNumber++;
        var resultLength = Marshal.SizeOf<T>();
        Span<byte> buffer = stackalloc byte[resultLength];
        while (true)
        {
            EnsureReadSize(resultLength);
            socket.ReceiveExact(buffer[0..32]);
            ref var baseHeader = ref buffer[0..32].AsStruct<ResponseHeader<byte>>();
            if (baseHeader.Verify(sequenceNumber))
            {
                socket.ReceiveExact(buffer[32..]);
                ref var responce = ref buffer.AsStruct<T>();
                if (responce.Verify(sequenceNumber))
                    return (buffer.ToStruct<T>(), null);
            }

            ref var content = ref buffer[0..32].AsStruct<XEvent>();
            if (content.EventType == EventType.Error)
            {
                sequenceNumber--;
                return (null, content.ErrorEvent);
            }

            bufferEvents.Push(buffer[0..32].ToStruct<XEvent>());
        }
    }


    void EnsureReadSize(int size)
    {
        while (true)
        {
            if (socket.Available >= size)
                break;
            socket.Poll(-1, SelectMode.SelectRead);
        }
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
                case EventType.KeyPress:
                case EventType.KeyRelease:
                case EventType.ButtonPress:
                case EventType.ButtonRelease:
                case EventType.MotionNotify:
                case EventType.EnterNotify:
                case EventType.LeaveNotify:
                case EventType.FocusIn:
                case EventType.FocusOut:
                case EventType.KeymapNotify:
                case EventType.Expose:
                case EventType.GraphicsExpose:
                case EventType.NoExpose:
                case EventType.VisibilityNotify:
                case EventType.CreateNotify:
                case EventType.DestroyNotify:
                case EventType.UnMapNotify:
                case EventType.MapNotify:
                case EventType.MapRequest:
                case EventType.ReParentNotify:
                case EventType.ConfigureNotify:
                case EventType.ConfigureRequest:
                case EventType.GravityNotify:
                case EventType.ResizeRequest:
                case EventType.CirculateNotify:
                case EventType.CirculateRequest:
                case EventType.PropertyNotify:
                case EventType.SelectionClear:
                case EventType.SelectionRequest:
                case EventType.SelectionNotify:
                case EventType.ColormapNotify:
                case EventType.ClientMessage:
                case EventType.MappingNotify:
                case EventType.GenericEvent:
                case EventType.LastEvent:
                default:
                    bufferEvents.Push(content);
                    break;
            }
        }

        return null;
    }
}