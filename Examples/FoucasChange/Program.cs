using Xcsb;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Response.Event;


// Connect to X server
var x = XcsbClient.Initialized();

// Get the first screen
var screen = x.HandshakeSuccessResponseBody.Screens[0];

// Define colors for focus states
uint colorFocused = 0xFF4444;   // Red when focused
uint colorUnfocused = 0x888888; // Gray when unfocused

// Create first window
var window1 = x.NewId();
x.CreateWindowUnchecked(screen.RootDepth.DepthValue, window1,
    screen.Root,
    50, 50, 300, 200,
    2,
    Xcsb.Models.ClassType.InputOutput,
    screen.RootVisualId,
    ValueMask.BackgroundPixel | ValueMask.EventMask,
    [colorFocused, (uint)(EventMask.KeyPressMask | EventMask.KeyReleaseMask |EventMask.ButtonPressMask
    | EventMask.FocusChangeMask | EventMask.ExposureMask | EventMask.PointerMotionMask)]
    );

x.MapWindowUnchecked(window1);

// Create second window
var window2 = x.NewId();
x.CreateWindowUnchecked(screen.RootDepth.DepthValue,
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
x.MapWindowUnchecked(window2);

Console.Write("Two windows created. Watch backgrounds change with focus!\n");
Console.Write("Gray = unfocused, Red = focused\n");
Console.Write("Click on windows or press Tab to switch focus\n");
Console.Write("Press 'q' to quit\n\n");

// Wait a moment for windows to be mapped
Thread.Sleep(1000);

// Initially set focus to first window
x.SetInputFocusUnchecked(Xcsb.Models.InputFocusMode.PointerRoot, window1, 0);
ChangeWindowColor(x, window1, colorFocused);
ChangeWindowColor(x, window2, colorUnfocused);

var gc = x.NewId();
x.CreateGCUnchecked(gc, window1, GCMask.Foreground | GCMask.Background, [screen.BlackPixel, screen.WhitePixel]);

// Track current focused window

var resultGetInputFocus = x.GetInputFocus();
var currentFocus = resultGetInputFocus.Focus;


var resultTranslateCoordinates = x.TranslateCoordinates(
    window1,
     window2,
    100, 100);
Console.WriteLine($"10, 10 trnsalate to  {resultTranslateCoordinates.DestinationX}, {resultTranslateCoordinates.DestinationY}"); ;
// Event loop to demonstrate focus changes
var isRunning = true;
while (isRunning)
{
    var evnt = x.GetEvent();

    Console.WriteLine(evnt.ReplyType);
    if (evnt.ReplyType == XEventType.LastEvent) return;
    switch (evnt.ReplyType)
    {
        case XEventType.KeyPress:
            {
                // Tab key (keycode 23) - switch focus between windows
                var keyPressEvent = evnt.As<KeyPressEvent>();
                if (keyPressEvent.Detail == 23)
                {
                    if (currentFocus == window1)
                    {
                        currentFocus = window2;
                        x.SetInputFocusUnchecked(Xcsb.Models.InputFocusMode.PointerRoot, window2, 0);
                        ChangeWindowColor(x, window1, colorUnfocused);
                        ChangeWindowColor(x, window2, colorFocused);
                    }
                    else
                    {
                        currentFocus = window1;
                        x.SetInputFocusUnchecked(Xcsb.Models.InputFocusMode.PointerRoot, window1, 0);
                        ChangeWindowColor(x, window1, colorFocused);
                        ChangeWindowColor(x, window2, colorUnfocused);
                    }
                }

                // 'q' key (keycode 24) - quit
                if (keyPressEvent.Detail == 24)
                {
                    isRunning = false;
                }
                break;
            }
        case XEventType.FocusIn:
            {
                var focusInEvent = evnt.As<FocusInEvent>();
                if (focusInEvent.Event == window1 || focusInEvent.Event == window2)
                {
                    ChangeWindowColor(x, (uint)focusInEvent.Event, colorFocused);
                    currentFocus = (uint)focusInEvent.Event;

                    // Set the other window to unfocused color
                    var otherWindow = (focusInEvent.Event == window1) ? window2 : window1;
                    ChangeWindowColor(x, otherWindow, colorUnfocused);
                }
                break;
            }
        case XEventType.FocusOut:
            {
                // Change to unfocused color when losing focus
                var focusOutEvent = evnt.As<FocusOutEvent>();
                if (focusOutEvent.Event == window1 || focusOutEvent.Event == window2)
                {
                    ChangeWindowColor(x, (uint)focusOutEvent.Event, colorUnfocused);
                }
                break;
            }
        case XEventType.ButtonPress:
            {
                // Set focus to the clicked window and update colors
                var buttonPressEvent = evnt.As<ButtonPressEvent>();
                x.SetInputFocusUnchecked(Xcsb.Models.InputFocusMode.PointerRoot, buttonPressEvent.EventWindow, 0);

                // Update colors immediately
                if (buttonPressEvent.EventWindow == window1)
                {
                    ChangeWindowColor(x, window1, colorFocused);
                    ChangeWindowColor(x, window2, colorUnfocused);
                    currentFocus = window1;
                }
                else if (buttonPressEvent.EventWindow == window2)
                {
                    ChangeWindowColor(x, window2, colorFocused);
                    ChangeWindowColor(x, window1, colorUnfocused);
                    currentFocus = window2;
                }
                break;
            }
        case XEventType.Expose:
            {
                // Redraw window contents when exposed
                var exposeEvent = evnt.As<ExposeEvent>();
                if (exposeEvent.Window == currentFocus)
                {
                    ChangeWindowColor(x, exposeEvent.Window, colorFocused);
                }
                else if (exposeEvent.Window == window1 || exposeEvent.Window == window2)
                {
                    ChangeWindowColor(x, exposeEvent.Window, colorUnfocused);
                }
                break;
            }
        case XEventType.MotionNotify:
            var motionEvent = evnt.As<MotionNotifyEvent>();
            var poient = new Xcsb.Models.Point(
                (ushort)motionEvent.EventX,
                (ushort)motionEvent.EventY);
            x.PolyPointUnchecked(Xcsb.Models.CoordinateMode.Origin, motionEvent.Window, gc, [poient]);
            if (motionEvent.Window == window1)
            {
                motionEvent.Window = window2;
                x.SendEventUnchecked(true, window2, (uint)EventMask.PointerMotionMask, evnt);
            }
            break;
    }
}

x.DestroyWindowUnchecked(window1);
x.DestroyWindowUnchecked(window2);
return;


static void ChangeWindowColor(IXProto x, uint win, uint color)
{
    x.ChangeWindowAttributesUnchecked(win, Xcsb.Masks.ValueMask.BackgroundPixel, [color]);
    x.ClearAreaUnchecked(false, win, 0, 0, 0, 0);
}