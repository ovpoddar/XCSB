using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
public struct XEvent
{
    [FieldOffset(0)] public EventType EventType;
    [FieldOffset(1)] public GenericError GenericError; 
    [FieldOffset(1)] public KeyPressEvent KeyPressEvent;
    [FieldOffset(1)] public KeyReleaseEvent KeyReleaseEvent;
    [FieldOffset(1)] public ButtonPressEvent ButtonPressEvent;
    [FieldOffset(1)] public ButtonReleaseEvent ButtonReleaseEvent;
    [FieldOffset(1)] public MotionNotifyEvent MotionNotifyEvent;
    [FieldOffset(1)] public EnterNotifyEvent EnterNotifyEvent;
    [FieldOffset(1)] public LeaveNotifyEvent LeaveNotifyEvent;
    [FieldOffset(1)] public FocusInEvent FocusInEvent;
    [FieldOffset(1)] public FocusOutEvent FocusOutEvent;
    [FieldOffset(1)] public KeymapNotifyEvent KeymapNotifyEvent;
    [FieldOffset(1)] public ExposeEvent ExposeEvent;
    [FieldOffset(1)] public GraphicsExposeEvent GraphicsExposeEvent;
    [FieldOffset(1)] public NoExposeEvent NoExposeEvent;
    [FieldOffset(1)] public VisibilityNotifyEvent VisibilityNotifyEvent;
    [FieldOffset(1)] public CreateNotifyEvent CreateNotifyEvent;
    [FieldOffset(1)] public DestroyNotifyEvent DestroyNotifyEvent;
    [FieldOffset(1)] public UnMapNotifyEvent UnMapNotifyEvent;
    [FieldOffset(1)] public MapNotifyEvent MapNotifyEvent;
    [FieldOffset(1)] public MapRequestEvent MapRequestEvent;
    [FieldOffset(1)] public ReParentNotifyEvent ReParentNotifyEvent;
    [FieldOffset(1)] public ConfigureNotifyEvent ConfigureNotifyEvent;
    [FieldOffset(1)] public ConfigureRequestEvent ConfigureRequestEvent;
    [FieldOffset(1)] public GravityNotifyEvent GravityNotifyEvent;
    [FieldOffset(1)] public ResizeRequestEvent ResizeRequestEvent;
    [FieldOffset(1)] public CirculateNotifyEvent CirculateNotifyEvent;
    [FieldOffset(1)] public CirculateRequestEvent CirculateRequestEvent;
    [FieldOffset(1)] public PropertyNotifyEvent PropertyNotifyEvent;
    [FieldOffset(1)] public SelectionClearEvent SelectionClearEvent;
    [FieldOffset(1)] public SelectionRequestEvent SelectionRequestEvent;
    [FieldOffset(1)] public SelectionNotifyEvent SelectionNotifyEvent;
    [FieldOffset(1)] public ColorMapNotifyEvent ColorMapNotifyEvent;
    [FieldOffset(1)] public ClientMessageEvent ClientMessageEvent;
    [FieldOffset(1)] public MappingNotifyEvent MappingNotifyEvent;
}