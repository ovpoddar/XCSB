using Xcsb;
using Xcsb.Event;
using Xcsb.Masks;


// Connect to X server
var x = XcsbClient.Initialized();

// Get the first screen
var screen = x.HandshakeSuccessResponseBody.Screens[0];

// Define colors for focus states
uint colorFocused = 0xFF4444;   // Red when focused
uint colorUnfocused = 0x888888; // Gray when unfocused

// Create first window
var window1 = x.NewId();
x.CreateWindow(screen.RootDepth.DepthValue, window1,
    screen.Root,
    50, 50, 300, 200,
    2,
    Xcsb.Models.ClassType.InputOutput,
    screen.RootVisualId,
    ValueMask.BackgroundPixel | ValueMask.EventMask,
    [colorFocused, (uint)(EventMask.KeyPressMask | EventMask.KeyReleaseMask |EventMask.ButtonPressMask
    | EventMask.FocusChangeMask | EventMask.ExposureMask | EventMask.PointerMotionMask)]
    );

x.MapWindow(window1);

// Create second window
var window2 = x.NewId();
x.CreateWindow(screen.RootDepth.DepthValue,
    window2,
    screen.Root,
    400, 50, 600, 500,
    2,
    Xcsb.Models.ClassType.InputOutput,
    screen.RootVisualId,
    ValueMask.BackgroundPixel | ValueMask.EventMask,
    [colorUnfocused, (uint)(
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
ChangeWindowColor(x, window1, colorFocused);
ChangeWindowColor(x, window2, colorUnfocused);

var gc = x.NewId();
x.CreateGC(gc, window1, GCMask.Foreground | GCMask.Background, [screen.BlackPixel, screen.WhitePixel]);

// Track current focused window

var resultGetInputFocus = x.GetInputFocus();
var currentFocus = resultGetInputFocus.Focus;

        
var resultTranslateCoordinates = x.TranslateCoordinates(
    window1,
     window2,
    100, 100);
Console.WriteLine($"10, 10 trnsalate to  {resultTranslateCoordinates.DestinationX}, {resultTranslateCoordinates.DestinationY}");;
// Event loop to demonstrate focus changes
var isRunning = true;
while (isRunning)
{
    var evnt = x.GetEvent();
    if (!evnt.HasValue) return;
    switch (evnt.Value.EventType)
    {
        case EventType.KeyPress:
            {
                // Tab key (keycode 23) - switch focus between windows
                if (evnt.Value.KeyPressEvent.Detail == 23)
                {
                    if (currentFocus == window1)
                    {
                        currentFocus = window2;
                        x.SetInputFocus(Xcsb.Models.InputFocusMode.PointerRoot, window2, 0);
                        ChangeWindowColor(x, window1, colorUnfocused);
                        ChangeWindowColor(x, window2, colorFocused);
                    }
                    else
                    {
                        currentFocus = window1;
                        x.SetInputFocus(Xcsb.Models.InputFocusMode.PointerRoot, window1, 0);
                        ChangeWindowColor(x, window1, colorFocused);
                        ChangeWindowColor(x, window2, colorUnfocused);
                    }
                }

                // 'q' key (keycode 24) - quit
                if (evnt.Value.KeyPressEvent.Detail == 24)
                {
                    isRunning = false;
                }
                break;
            }
        case EventType.FocusIn:
            {
                if (evnt.Value.FocusInEvent.Event == window1 || evnt.Value.FocusInEvent.Event == window2)
                {
                    ChangeWindowColor(x, (uint)evnt.Value.FocusInEvent.Event, colorFocused);
                    currentFocus = (uint)evnt.Value.FocusInEvent.Event;

                    // Set the other window to unfocused color
                    var otherWindow = (evnt.Value.FocusInEvent.Event == window1) ? window2 : window1;
                    ChangeWindowColor(x, otherWindow, colorUnfocused);
                }
                break;
            }
        case EventType.FocusOut:
            {
                // Change to unfocused color when losing focus
                if (evnt.Value.FocusOutEvent.Event == window1 || evnt.Value.FocusOutEvent.Event == window2)
                {
                    ChangeWindowColor(x, (uint)evnt.Value.FocusOutEvent.Event, colorUnfocused);
                }
                break;
            }
        case EventType.ButtonPress:
            {
                // Set focus to the clicked window and update colors
                x.SetInputFocus(Xcsb.Models.InputFocusMode.PointerRoot, evnt.Value.ButtonPressEvent.EventWindow, 0);

                // Update colors immediately
                if (evnt.Value.ButtonPressEvent.EventWindow == window1)
                {
                    ChangeWindowColor(x, window1, colorFocused);
                    ChangeWindowColor(x, window2, colorUnfocused);
                    currentFocus = window1;
                }
                else if (evnt.Value.ButtonPressEvent.EventWindow == window2)
                {
                    ChangeWindowColor(x, window2, colorFocused);
                    ChangeWindowColor(x, window1, colorUnfocused);
                    currentFocus = window2;
                }
                break;
            }
        case EventType.Expose:
            {
                // Redraw window contents when exposed
                if (evnt.Value.ExposeEvent.Window == currentFocus)
                {
                    ChangeWindowColor(x, evnt.Value.ExposeEvent.Window, colorFocused);
                }
                else if (evnt.Value.ExposeEvent.Window == window1 || evnt.Value.ExposeEvent.Window == window2)
                {
                    ChangeWindowColor(x, evnt.Value.ExposeEvent.Window, colorUnfocused);
                }
                break;
            }
        case EventType.MotionNotify:
            var poient = new Xcsb.Models.Point(
                (ushort)evnt.Value.MotionNotifyEvent.EventX,
                (ushort)evnt.Value.MotionNotifyEvent.EventY);
            x.PolyPoint(Xcsb.Models.CoordinateMode.Origin, evnt.Value.MotionNotifyEvent.Window, gc, [poient]);
            if (evnt.Value.MotionNotifyEvent.Window == window1)
            {
                var evn = evnt.Value;
                evn.MotionNotifyEvent.Window = window2;
                x.SendEvent(true, window2, (uint)EventMask.PointerMotionMask, evn);
            }
            break;
    }
}

x.DestroyWindow(window1);
x.DestroyWindow(window2);
return;


static void ChangeWindowColor(IXProto x, uint win, uint color)
{
    x.ChangeWindowAttributes(win, Xcsb.Masks.ValueMask.BackgroundPixel, [color]);
    x.ClearArea(false, win, 0, 0, 0, 0);
}