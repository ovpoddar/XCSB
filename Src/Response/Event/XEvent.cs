using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
public unsafe struct XEvent : IXEvent
{
    [FieldOffset(0)] public readonly EventType Reply;
    [FieldOffset(1)] private readonly byte Value;
    [FieldOffset(2)] public readonly ushort Sequence;

    [FieldOffset(0)] private fixed byte _data[32];

    public readonly ref T As<T>() where T : struct
    {
        var isNotValid = this.Reply switch
        {
            EventType.KeyPress when typeof(T) == typeof(KeyPressEvent) => false,
            EventType.KeyRelease when typeof(T) == typeof(KeyReleaseEvent) => false,
            EventType.ButtonPress when typeof(T) == typeof(ButtonPressEvent) => false,
            EventType.ButtonRelease when typeof(T) == typeof(ButtonReleaseEvent) => false,
            EventType.MotionNotify when typeof(T) == typeof(MotionNotifyEvent) => false,
            EventType.EnterNotify when typeof(T) == typeof(EnterNotifyEvent) => false,
            EventType.LeaveNotify when typeof(T) == typeof(LeaveNotifyEvent) => false,
            EventType.FocusIn when typeof(T) == typeof(FocusInEvent) => false,
            EventType.FocusOut when typeof(T) == typeof(FocusOutEvent) => false,
            EventType.KeymapNotify when typeof(T) == typeof(KeymapNotifyEvent) => false,
            EventType.Expose when typeof(T) == typeof(ExposeEvent) => false,
            EventType.GraphicsExpose when typeof(T) == typeof(GraphicsExposeEvent) => false,
            EventType.NoExpose when typeof(T) == typeof(NoExposeEvent) => false,
            EventType.VisibilityNotify when typeof(T) == typeof(VisibilityNotifyEvent) => false,
            EventType.CreateNotify when typeof(T) == typeof(CreateNotifyEvent) => false,
            EventType.DestroyNotify when typeof(T) == typeof(DestroyNotifyEvent) => false,
            EventType.UnMapNotify when typeof(T) == typeof(UnMapNotifyEvent) => false,
            EventType.MapNotify when typeof(T) == typeof(MapNotifyEvent) => false,
            EventType.MapRequest when typeof(T) == typeof(MapRequestEvent) => false,
            EventType.ReParentNotify when typeof(T) == typeof(ReParentNotifyEvent) => false,
            EventType.ConfigureNotify when typeof(T) == typeof(ConfigureNotifyEvent) => false,
            EventType.ConfigureRequest when typeof(T) == typeof(ConfigureRequestEvent) => false,
            EventType.GravityNotify when typeof(T) == typeof(GravityNotifyEvent) => false,
            EventType.ResizeRequest when typeof(T) == typeof(ResizeRequestEvent) => false,
            EventType.CirculateNotify when typeof(T) == typeof(CirculateNotifyEvent) => false,
            EventType.CirculateRequest when typeof(T) == typeof(CirculateRequestEvent) => false,
            EventType.PropertyNotify when typeof(T) == typeof(PropertyNotifyEvent) => false,
            EventType.SelectionClear when typeof(T) == typeof(SelectionClearEvent) => false,
            EventType.SelectionRequest when typeof(T) == typeof(SelectionRequestEvent) => false,
            EventType.SelectionNotify when typeof(T) == typeof(SelectionNotifyEvent) => false,
            EventType.ColormapNotify when typeof(T) == typeof(ColorMapNotifyEvent) => false,
            EventType.ClientMessage when typeof(T) == typeof(ClientMessageEvent) => false,
            EventType.MappingNotify when typeof(T) == typeof(MappingNotifyEvent) => false,
            _ => true
        };

        if (isNotValid)
            throw new InvalidOperationException();

        fixed (byte* ptr = this._data)
            return ref new Span<byte>(ptr, 32).AsStruct<T>();
    }


    public bool Verify(in int sequence)
    {
        if (this.Sequence != sequence
            && this.Reply is > EventType.KeyPress and < EventType.LastEvent)
            return false;

#if NETSTANDARD
        if (!Enum.IsDefined(typeof(EventType), Reply)) return false;
#else
        if (!Enum.IsDefined(Reply)) return false;
#endif
        fixed (byte* ptr = this._data)
        {
            return Reply switch
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
                _ => false
            };
        }
    }
}