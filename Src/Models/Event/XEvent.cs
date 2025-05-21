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

    [FieldOffset(0)]
    public InputEvent InputEvent;

    [FieldOffset(0)]
    public MotionEvent MotionEvent;

}
