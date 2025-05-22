using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Src.Models.Event;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
public struct XEvent
{
    [FieldOffset(0)]
    public EventType EventType;

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
}
