using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Helpers;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
public unsafe struct XGenericEvent : IXEvent
{
    [FieldOffset(0)] public readonly ResponseHeader<byte> ResponseHeader;
    [FieldOffset(4)] public fixed byte Data[28];


    [FieldOffset(0)] private fixed byte _data[32];

    public readonly bool Verify(in int sequence)
    {
        if (this.ResponseHeader.Sequence != sequence
            && this.ResponseHeader.Reply is > ResponseType.KeyPress and < ResponseType.LastEvent)
            return false;

        var errorType = (EventType)this.ResponseHeader.GetValue();
#if NETSTANDARD
         if(!Enum.IsDefined(typeof(EventType), errorType)) return false;
#else
        if (!Enum.IsDefined(errorType)) return false;
#endif
        fixed (byte* ptr = this._data)
        {
            return errorType switch
            {
                EventType.KeyPress => new Span<byte>(ptr, 32).AsStruct<KeyPressEvent>().Verify(in sequence),
                EventType.KeyRelease => new Span<byte>(ptr, 32).AsStruct<KeyReleaseEvent>().Verify(in sequence),
                EventType.ButtonPress => new Span<byte>(ptr, 32).AsStruct<ButtonPressEvent>().Verify(in sequence),
                EventType.ButtonRelease => new Span<byte>(ptr, 32).AsStruct<ButtonReleaseEvent>().Verify(in sequence),
                EventType.MotionNotify => new Span<byte>(ptr, 32).AsStruct<MotionNotifyEvent>().Verify(in sequence),
                EventType.EnterNotify => new Span<byte>(ptr, 32).AsStruct<EnterNotifyEvent>().Verify(in sequence),
                EventType.LeaveNotify => new Span<byte>(ptr, 32).AsStruct<LeaveNotifyEvent>().Verify(in sequence),
                EventType.FocusIn => new Span<byte>(ptr, 32).AsStruct<FocusInEvent>().Verify(in sequence),
                EventType.FocusOut => new Span<byte>(ptr, 32).AsStruct<FocusOutEvent>().Verify(in sequence),
                EventType.KeymapNotify => new Span<byte>(ptr, 32).AsStruct<KeymapNotifyEvent>().Verify(in sequence),
                EventType.Expose => new Span<byte>(ptr, 32).AsStruct<ExposeEvent>().Verify(in sequence),
                EventType.GraphicsExpose => new Span<byte>(ptr, 32).AsStruct<GraphicsExposeEvent>().Verify(in sequence),
                EventType.NoExpose => new Span<byte>(ptr, 32).AsStruct<NoExposeEvent>().Verify(in sequence),
                EventType.VisibilityNotify => new Span<byte>(ptr, 32).AsStruct<VisibilityNotifyEvent>()
                    .Verify(in sequence),
                EventType.CreateNotify => new Span<byte>(ptr, 32).AsStruct<CreateNotifyEvent>().Verify(in sequence),
                EventType.DestroyNotify => new Span<byte>(ptr, 32).AsStruct<DestroyNotifyEvent>().Verify(in sequence),
                EventType.UnMapNotify => new Span<byte>(ptr, 32).AsStruct<UnMapNotifyEvent>().Verify(in sequence),
                EventType.MapNotify => new Span<byte>(ptr, 32).AsStruct<MapNotifyEvent>().Verify(in sequence),
                EventType.MapRequest => new Span<byte>(ptr, 32).AsStruct<MapRequestEvent>().Verify(in sequence),
                EventType.ReParentNotify => new Span<byte>(ptr, 32).AsStruct<ReParentNotifyEvent>().Verify(in sequence),
                EventType.ConfigureNotify => new Span<byte>(ptr, 32).AsStruct<ConfigureNotifyEvent>()
                    .Verify(in sequence),
                EventType.ConfigureRequest => new Span<byte>(ptr, 32).AsStruct<ConfigureRequestEvent>()
                    .Verify(in sequence),
                EventType.GravityNotify => new Span<byte>(ptr, 32).AsStruct<GravityNotifyEvent>().Verify(in sequence),
                EventType.ResizeRequest => new Span<byte>(ptr, 32).AsStruct<ResizeRequestEvent>().Verify(in sequence),
                EventType.CirculateNotify => new Span<byte>(ptr, 32).AsStruct<CirculateNotifyEvent>()
                    .Verify(in sequence),
                EventType.CirculateRequest => new Span<byte>(ptr, 32).AsStruct<CirculateRequestEvent>()
                    .Verify(in sequence),
                EventType.PropertyNotify => new Span<byte>(ptr, 32).AsStruct<PropertyNotifyEvent>().Verify(in sequence),
                EventType.SelectionClear => new Span<byte>(ptr, 32).AsStruct<SelectionClearEvent>().Verify(in sequence),
                EventType.SelectionRequest => new Span<byte>(ptr, 32).AsStruct<SelectionRequestEvent>()
                    .Verify(in sequence),
                EventType.SelectionNotify => new Span<byte>(ptr, 32).AsStruct<SelectionNotifyEvent>()
                    .Verify(in sequence),
                EventType.ColormapNotify => new Span<byte>(ptr, 32).AsStruct<ColorMapNotifyEvent>().Verify(in sequence),
                EventType.ClientMessage => new Span<byte>(ptr, 32).AsStruct<ClientMessageEvent>().Verify(in sequence),
                EventType.MappingNotify => new Span<byte>(ptr, 32).AsStruct<MappingNotifyEvent>().Verify(in sequence),
                EventType.GenericEvent => new Span<byte>(ptr, 32).AsStruct<XGenericEvent>().Verify(in sequence),
                _ => false
            };
        }
    }
}