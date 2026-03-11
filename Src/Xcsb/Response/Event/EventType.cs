using Xcsb.Connection.Models;

namespace Xcsb.Response.Event;

public sealed record EventType : XEventType
{
    private EventType(byte value, string name) : base(value, name) { }

    public static readonly EventType KeyPress = new EventType(2, "KeyPress");
    public static readonly EventType KeyRelease = new EventType(3, "KeyRelease");
    public static readonly EventType ButtonPress = new EventType(4, "ButtonPress");
    public static readonly EventType ButtonRelease = new EventType(5, "ButtonRelease");
    public static readonly EventType MotionNotify = new EventType(6, "MotionNotify");
    public static readonly EventType EnterNotify = new EventType(7, "EnterNotify");
    public static readonly EventType LeaveNotify = new EventType(8, "LeaveNotify");
    public static readonly EventType FocusIn = new EventType(9, "FocusIn");
    public static readonly EventType FocusOut = new EventType(10, "FocusOut");
    public static readonly EventType KeymapNotify = new EventType(11, "KeymapNotify");
    public static readonly EventType Expose = new EventType(12, "Expose");
    public static readonly EventType GraphicsExpose = new EventType(13, "GraphicsExpose");
    public static readonly EventType NoExpose = new EventType(14, "NoExpose");
    public static readonly EventType VisibilityNotify = new EventType(15, "VisibilityNotify");
    public static readonly EventType CreateNotify = new EventType(16, "CreateNotify");
    public static readonly EventType DestroyNotify = new EventType(17, "DestroyNotify");
    public static readonly EventType UnMapNotify = new EventType(18, "UnMapNotify");
    public static readonly EventType MapNotify = new EventType(19, "MapNotify");
    public static readonly EventType MapRequest = new EventType(20, "MapRequest");
    public static readonly EventType ReParentNotify = new EventType(21, "ReParentNotify");
    public static readonly EventType ConfigureNotify = new EventType(22, "ConfigureNotify");
    public static readonly EventType ConfigureRequest = new EventType(23, "ConfigureRequest");
    public static readonly EventType GravityNotify = new EventType(24, "GravityNotify");
    public static readonly EventType ResizeRequest = new EventType(25, "ResizeRequest");
    public static readonly EventType CirculateNotify = new EventType(26, "CirculateNotify");
    public static readonly EventType CirculateRequest = new EventType(27, "CirculateRequest");
    public static readonly EventType PropertyNotify = new EventType(28, "PropertyNotify");
    public static readonly EventType SelectionClear = new EventType(29, "SelectionClear");
    public static readonly EventType SelectionRequest = new EventType(30, "SelectionRequest");
    public static readonly EventType SelectionNotify = new EventType(31, "SelectionNotify");
    public static readonly EventType ColormapNotify = new EventType(32, "ColormapNotify");
    public static readonly EventType ClientMessage = new EventType(33, "ClientMessage");
    public static readonly EventType MappingNotify = new EventType(34, "MappingNotify");
    public static readonly EventType LastEvent = new EventType(36, "LastEvent");
}