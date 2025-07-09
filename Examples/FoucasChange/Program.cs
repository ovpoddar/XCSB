using System.Runtime.InteropServices;
using Xcsb;
using Xcsb.Masks;
using Xcsb.Models.Event;


// Connect to X server
var x = XcsbClient.Initialized();

// Get the first screen
var screen = x.HandshakeSuccessResponseBody.Screens[0];

// Define colors for focus states
uint color_focused = 0xFF4444;   // Red when focused
uint color_unfocused = 0x888888; // Gray when unfocused

// Create first window
var window1 = x.NewId();
x.CreateWindow(screen.RootDepth.DepthValue, window1,
    screen.Root,
    50, 50, 300, 200,
    2,
    Xcsb.Models.ClassType.InputOutput,
    screen.RootVisualId,
    ValueMask.BackgroundPixel | ValueMask.EventMask,
    [color_focused, (uint)(EventMask.KeyPressMask | EventMask.KeyReleaseMask |EventMask.ButtonPressMask
    | EventMask.FocusChangeMask | EventMask.ExposureMask | EventMask.PointerMotionMask)]
    );

x.MapWindow(window1);

// Create second window
var window2 = x.NewId();
x.CreateWindow(screen.RootDepth.DepthValue,
    window2,
    screen.Root,
    400, 50, 300, 200,
    2,
    Xcsb.Models.ClassType.InputOutput,
    screen.RootVisualId,
    ValueMask.BackgroundPixel | ValueMask.EventMask,
    [color_unfocused, (uint)(
        EventMask.KeyPressMask | EventMask.KeyReleaseMask | EventMask.ButtonPressMask|
        EventMask.FocusChangeMask | EventMask.ExposureMask | EventMask.PointerMotionMask)]
    );


// Map both windows
x.MapWindow(window2);

Console.Write("Two windows created. Watch backgrounds change with focus!\n");
Console.Write("Gray = unfocused, Red = focused\n");
Console.Write("Click on windows or press Tab to switch focus\n");
Console.Write("Press 'q' to quit\n\n");

// Wait a moment for windows to be mapped
Thread.Sleep(1000);

// Initially set focus to first window
x.SetInputFocus(Xcsb.Models.InputFocusMode.PointerRoot, window1, 0);
ChangeWindowColor(x, window1, color_focused);
ChangeWindowColor(x, window2, color_unfocused);

var gc = x.NewId();
x.CreateGC(gc, window1, GCMask.Foreground | GCMask.Background, [screen.BlackPixel, screen.WhitePixel]);

// Track current focused window
var current_focus = window1;

// Event loop to demonstrate focus changes
var isRunning = true;
while (isRunning)
{
    var evnt = x.GetEvent();
    switch (evnt.EventType)
    {
        case EventType.KeyPress:
            {
                // Tab key (keycode 23) - switch focus between windows
                if (evnt.InputEvent.Detail == 23)
                {
                    if (current_focus == window1)
                    {
                        current_focus = window2;
                        x.SetInputFocus(Xcsb.Models.InputFocusMode.PointerRoot, window2, 0);
                        ChangeWindowColor(x, window1, color_unfocused);
                        ChangeWindowColor(x, window2, color_focused);
                    }
                    else
                    {
                        current_focus = window1;
                        x.SetInputFocus(Xcsb.Models.InputFocusMode.PointerRoot, window1, 0);
                        ChangeWindowColor(x, window1, color_focused);
                        ChangeWindowColor(x, window2, color_unfocused);
                    }
                }

                // 'q' key (keycode 24) - quit
                if (evnt.InputEvent.Detail == 24)
                {
                    isRunning = false;
                }
                break;
            }
        case EventType.FocusIn:
            {
                // Update background color when focus changes

                if (evnt.FocusEvent.Event == window1 || evnt.FocusEvent.Event == window2)
                {
                    ChangeWindowColor(x, (uint)evnt.FocusEvent.Event, color_focused);
                    current_focus = (uint)evnt.FocusEvent.Event;

                    // Set the other window to unfocused color
                    var other_window = (evnt.FocusEvent.Event == window1) ? window2 : window1;
                    ChangeWindowColor(x, other_window, color_unfocused);
                }
                break;
            }
        case EventType.FocusOut:
            {
                // Change to unfocused color when losing focus
                if (evnt.FocusEvent.Event == window1 || evnt.FocusEvent.Event == window2)
                {
                    ChangeWindowColor(x, (uint)evnt.FocusEvent.Event, color_unfocused);
                }
                break;
            }
        case EventType.ButtonPress:
            {
                // Set focus to the clicked window and update colors
                x.SetInputFocus(Xcsb.Models.InputFocusMode.PointerRoot, evnt.InputEvent.EventWindow, 0);

                // Update colors immediately
                if (evnt.InputEvent.EventWindow == window1)
                {
                    ChangeWindowColor(x, window1, color_focused);
                    ChangeWindowColor(x, window2, color_unfocused);
                    current_focus = window1;
                }
                else if (evnt.InputEvent.EventWindow == window2)
                {
                    ChangeWindowColor(x, window2, color_focused);
                    ChangeWindowColor(x, window1, color_unfocused);
                    current_focus = window2;
                }
                break;
            }
        case EventType.Expose:
            {
                // Redraw window contents when exposed
                if (evnt.ExposeEvent.Window == current_focus)
                {
                    ChangeWindowColor(x, evnt.ExposeEvent.Window, color_focused);
                }
                else if (evnt.ExposeEvent.Window == window1 || evnt.ExposeEvent.Window == window2)
                {
                    ChangeWindowColor(x, evnt.ExposeEvent.Window, color_unfocused);
                }
                break;
            }
        case EventType.MotionNotify:
            var poient = new Xcsb.Models.Point(
                (ushort)evnt.MotionEvent.EventX,
                (ushort)evnt.MotionEvent.EventY);
            x.PolyPoint(Xcsb.Models.CoordinateMode.Origin, evnt.MotionEvent.Window, gc, [poient]);
            if (evnt.MotionEvent.Window == window1)
            {
                var evn = evnt;
                evn.MotionEvent.Window = window2;
                x.SendEvent(true, window2, (uint)EventMask.PointerMotionMask, evn);
            }
            break;
    }
}

x.DestroyWindow(window1);
x.DestroyWindow(window2);


static void ChangeWindowColor(IXProto x, uint win, uint color)
{
    x.ChangeWindowAttributes(win, Xcsb.Masks.ValueMask.BackgroundPixel, [color]);
    x.ClearArea(false, win, 0, 0, 0, 0);
}