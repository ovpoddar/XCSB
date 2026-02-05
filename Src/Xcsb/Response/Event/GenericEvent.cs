using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
public unsafe struct GenericEvent : IXEvent
{
    [FieldOffset(0)] public readonly EventType Reply;
    [FieldOffset(1)] private readonly byte Value;
    [FieldOffset(2)] public readonly ushort Sequence;

    [FieldOffset(0)] private fixed byte _data[32];

    internal readonly ref readonly T As<T>() where T : struct
    {
        var isNotValid = Reply switch
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
            _ when typeof(T) == typeof(GenericEvent) => false,
            _ when typeof(T) == typeof(XEvent) => false,
            _ => true
        };

        if (Reply is > EventType.KeyPress or not < EventType.LastEvent && isNotValid)
            throw new InvalidCastException();

        fixed (byte* ptr = _data)
            return ref new Span<byte>(ptr, 32).AsStruct<T>();
    }


    public bool Verify()
    {
        fixed (byte* ptr = _data)
        {
            return Reply switch
            {
                EventType.KeyPress => new Span<byte>(ptr, 32).AsStruct<KeyPressEvent>().Verify(),
                EventType.KeyRelease => new Span<byte>(ptr, 32).AsStruct<KeyReleaseEvent>().Verify(),
                EventType.ButtonPress => new Span<byte>(ptr, 32).AsStruct<ButtonPressEvent>().Verify(),
                EventType.ButtonRelease => new Span<byte>(ptr, 32).AsStruct<ButtonReleaseEvent>().Verify(),
                EventType.MotionNotify => new Span<byte>(ptr, 32).AsStruct<MotionNotifyEvent>().Verify(),
                EventType.EnterNotify => new Span<byte>(ptr, 32).AsStruct<EnterNotifyEvent>().Verify(),
                EventType.LeaveNotify => new Span<byte>(ptr, 32).AsStruct<LeaveNotifyEvent>().Verify(),
                EventType.FocusIn => new Span<byte>(ptr, 32).AsStruct<FocusInEvent>().Verify(),
                EventType.FocusOut => new Span<byte>(ptr, 32).AsStruct<FocusOutEvent>().Verify(),
                EventType.KeymapNotify => new Span<byte>(ptr, 32).AsStruct<KeymapNotifyEvent>().Verify(),
                EventType.Expose => new Span<byte>(ptr, 32).AsStruct<ExposeEvent>().Verify(),
                EventType.GraphicsExpose => new Span<byte>(ptr, 32).AsStruct<GraphicsExposeEvent>().Verify(),
                EventType.NoExpose => new Span<byte>(ptr, 32).AsStruct<NoExposeEvent>().Verify(),
                EventType.VisibilityNotify => new Span<byte>(ptr, 32).AsStruct<VisibilityNotifyEvent>()
                    .Verify(),
                EventType.CreateNotify => new Span<byte>(ptr, 32).AsStruct<CreateNotifyEvent>().Verify(),
                EventType.DestroyNotify => new Span<byte>(ptr, 32).AsStruct<DestroyNotifyEvent>().Verify(),
                EventType.UnMapNotify => new Span<byte>(ptr, 32).AsStruct<UnMapNotifyEvent>().Verify(),
                EventType.MapNotify => new Span<byte>(ptr, 32).AsStruct<MapNotifyEvent>().Verify(),
                EventType.MapRequest => new Span<byte>(ptr, 32).AsStruct<MapRequestEvent>().Verify(),
                EventType.ReParentNotify => new Span<byte>(ptr, 32).AsStruct<ReParentNotifyEvent>().Verify(),
                EventType.ConfigureNotify => new Span<byte>(ptr, 32).AsStruct<ConfigureNotifyEvent>()
                    .Verify(),
                EventType.ConfigureRequest => new Span<byte>(ptr, 32).AsStruct<ConfigureRequestEvent>()
                    .Verify(),
                EventType.GravityNotify => new Span<byte>(ptr, 32).AsStruct<GravityNotifyEvent>().Verify(),
                EventType.ResizeRequest => new Span<byte>(ptr, 32).AsStruct<ResizeRequestEvent>().Verify(),
                EventType.CirculateNotify => new Span<byte>(ptr, 32).AsStruct<CirculateNotifyEvent>()
                    .Verify(),
                EventType.CirculateRequest => new Span<byte>(ptr, 32).AsStruct<CirculateRequestEvent>()
                    .Verify(),
                EventType.PropertyNotify => new Span<byte>(ptr, 32).AsStruct<PropertyNotifyEvent>().Verify(),
                EventType.SelectionClear => new Span<byte>(ptr, 32).AsStruct<SelectionClearEvent>().Verify(),
                EventType.SelectionRequest => new Span<byte>(ptr, 32).AsStruct<SelectionRequestEvent>()
                    .Verify(),
                EventType.SelectionNotify => new Span<byte>(ptr, 32).AsStruct<SelectionNotifyEvent>()
                    .Verify(),
                EventType.ColormapNotify => new Span<byte>(ptr, 32).AsStruct<ColorMapNotifyEvent>().Verify(),
                EventType.ClientMessage => new Span<byte>(ptr, 32).AsStruct<ClientMessageEvent>().Verify(),
                EventType.MappingNotify => new Span<byte>(ptr, 32).AsStruct<MappingNotifyEvent>().Verify(),
                _ => true
            };
        }
    }
}