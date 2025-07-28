using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models.Event;
using Xcsb.Models.Response;

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
        if (socket.Available == 0)
            socket.Poll(-1, SelectMode.SelectRead);
        // todo: this is not exhausting all the data
        // so later call might fail
        Span<byte> buffer = stackalloc byte[Marshal.SizeOf<T>()];
        while (socket.Available != 0)
        {
            buffer.Clear();
            socket.ReceiveExact(buffer);
            ref var content = ref buffer.AsStruct<XEvent>();
            switch (content)
            {
                case { EventType: EventType.Error }:
                    return (null, content.ErrorEvent);
                case { EventType: (EventType)1 }:
                    var result = buffer.AsStruct<T>();
                    return result.Verify()
                        ? (result, null)
                        : (null, null); // todo: fix the reporting
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