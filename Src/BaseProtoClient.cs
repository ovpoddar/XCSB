using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models.Event;
using Xcsb.Models.Response;
using Xcsb.Models.Response.Contract;

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
        Debug.Assert(Marshal.SizeOf<T>() == 32);
        sequenceNumber++;
        Span<byte> buffer = stackalloc byte[32];
        while (true)
        {
            EnsureReadSize(32);
            socket.ReceiveExact(buffer);
            ref var baseHeader = ref buffer.AsStruct<T>();
            if (baseHeader.Verify(sequenceNumber))
                return (buffer.ToStruct<T>(), null);

            ref var content = ref buffer.AsStruct<XEvent>();
            if (content.EventType == EventType.Error)
            {
                sequenceNumber--;
                return (null, content.ErrorEvent);
            }
            bufferEvents.Push(buffer.ToStruct<XEvent>());
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