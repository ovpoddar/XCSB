using System.Runtime.InteropServices;

namespace Xcsb.Models.Event;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
public struct XEvent
{
    [FieldOffset(0)]
    public EventType EventType;

    [FieldOffset(1)]
    public ErrorEvent ErrorEvent;

    [FieldOffset(1)]
    public InputEvent InputEvent;

    [FieldOffset(1)]
    public MotionEvent MotionEvent;

    [FieldOffset(1)]
    public EnterEvent EnterEvent;

    [FieldOffset(1)]
    public FocusEvent FocusEvent;

    [FieldOffset(1)]
    public KeymapEvent KeymapEvent;

    [FieldOffset(1)]
    public ExposeEvent ExposeEvent;

    [FieldOffset(1)]
    public GraphicsExposeEvent GraphicsExposeEvent;

    [FieldOffset(1)]
    public NoExposeEvent NoExposeEvent;

    [FieldOffset(1)]
    public VisibilityNotifyEvent VisibilityNotifyEvent;

    [FieldOffset(1)]
    public CreateNotifyEvent CreateNotifyEvent;

    [FieldOffset(1)]
    public DestroyNotifyEvent DestroyNotifyEvent;

    [FieldOffset(1)]
    public UnMapNotifyEvent UnMapNotifyEvent;

    [FieldOffset(1)]
    public MapNotifyEvent MapNotifyEvent;

    [FieldOffset(1)]
    public MapRequestEvent MapRequestEvent;

    [FieldOffset(1)]
    public ReParentNotifyEvent ReParentNotifyEvent;

    [FieldOffset(1)]
    public ConfigureNotifyEvent ConfigureNotifyEvent;

    [FieldOffset(1)]
    public ConfigureRequestEvent ConfigureRequestEvent;

    [FieldOffset(1)]
    public GravityNotifyEvent GravityNotifyEvent;

    [FieldOffset(1)]
    public ResizeRequestEvent ResizeRequestEvent;

    [FieldOffset(1)]
    public CirculateEvent CirculateEvent;

    [FieldOffset(1)]
    public PropertyNotifyEvent PropertyNotifyEvent;

    [FieldOffset(1)]
    public SelectionClearEvent SelectionClearEvent;

    [FieldOffset(1)]
    public SelectionRequestEvent SelectionRequestEvent;

    [FieldOffset(1)]
    public SelectionNotifyEvent SelectionNotifyEvent;

    [FieldOffset(1)]
    public ColorMapNotifyEvent ColorMapNotifyEvent;

    [FieldOffset(1)]
    public ClientMessageEvent ClientMessageEvent;

    [FieldOffset(1)]
    public MappingNotifyEvent MappingNotifyEvent;

    [FieldOffset(1)]
    public GenericEvent GenericEvent;
}
