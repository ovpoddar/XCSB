using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Event;

using var c = XcsbClient.Initialized();

var window = c.NewId();
c.CreateWindow(
    window,
    c.HandshakeSuccessResponseBody.Screens[0].Root,
 0,
 0,
 500,
 500,
 0,
 ClassType.InputOutput,
 c.HandshakeSuccessResponseBody.Screens[0].RootVisualId,
 ValueMask.BackgroundPixel | ValueMask.EventMask,
 [0x00ffffff, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask)]
);

c.MapWindow(window);
var isRunning = true;
Span<byte> eventBytes = stackalloc byte[Xcsb.XcsbClient.GetEventSize()];
Debug.Assert(eventBytes.Length == 32);

var fontId = c.NewId();
c.OpenFont("-misc-fixed-*-*-*-*-13-*-*-*-*-*-iso10646-1", fontId);
var fontId1 = c.NewId();
c.OpenFont("fixed", fontId1);

var gc = c.NewId();
c.CreateGC(gc, window, GCMask.Foreground | GCMask.Background | GCMask.Font, [c.HandshakeSuccessResponseBody.Screens[0].BlackPixel, c.HandshakeSuccessResponseBody.Screens[0].WhitePixel, fontId]);
var gc1 = c.NewId();
c.CreateGC(gc1, window, GCMask.Foreground | GCMask.Background | GCMask.Font, [c.HandshakeSuccessResponseBody.Screens[0].BlackPixel, c.HandshakeSuccessResponseBody.Screens[0].WhitePixel, fontId1]);
var isExecuted = false;

while (isRunning)
{
    var Event = c.GetEvent(eventBytes);
    if (Event.EventType == EventType.Error)
    {
        Console.WriteLine(Event.ErrorEvent.ErrorCode.ToString());
        isRunning = false;
        break;
    }
    else if (Event.EventType is EventType.KeyPress or EventType.ButtonPress)
    {
        if (!isExecuted)
        {
            c.ChangeWindowAttributes(window,
                ValueMask.BackgroundPixel | ValueMask.EventMask,
                [0x00ffffff, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask | EventMask.ButtonPressMask)]);
            isExecuted = true;
        }
        if (Event.InputEvent.Detail == 24)
        {
            c.DestroyWindow(window);
            isRunning = false;
        }
        if (Event.InputEvent.Detail == 46)
        {
            c.CirculateWindow(Direction.LowerHighest, window);
        }
        c.Bell(100);
        Console.WriteLine($"event {Event.EventType} {Event.InputEvent.Detail}");
    }
    else if (Event.EventType == EventType.Expose)
    {
        c.ImageText16(window, gc, 10, 15, "this is a utf 16 string");
        c.ImageText8(window, gc, 10, 40, "this is a utf 8 string"u8);
    }
    else
    {
        Console.WriteLine(Event.EventType);
    }
}