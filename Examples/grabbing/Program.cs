using Xcsb;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Response.Event;

var connection = XcsbClient.Initialized();
var screen = connection.HandshakeSuccessResponseBody.Screens[0];
var colormap = screen.DefaultColormap;
var window = connection.NewId();
var demoActive = false;
connection.CreateWindowUnchecked(0, window, screen.Root,
    100, 100, 500, 400, 2, Xcsb.Models.ClassType.InputOutput,
    screen.RootVisualId, ValueMask.BackgroundPixel | ValueMask.EventMask,
    [
        screen.WhitePixel,
        (uint)(EventMask.KeyPressMask | EventMask.ExposureMask | EventMask.ButtonPressMask |
               EventMask.ButtonReleaseMask)
    ]);
connection.ChangePropertyUnchecked<byte>(PropertyMode.Replace, window,
    (ATOM)39, ATOM.String, "XCB System Control Demo"u8.ToArray());
var gc = connection.NewId();
connection.CreateGCUnchecked(gc, window, 0, []);
connection.MapWindowUnchecked(window);
show_help();

bool isRunning = true;
while (isRunning)
{
    var evnt = connection.GetEvent();
    if (evnt.ReplyType == XEventType.LastEvent) return 0;
    if (evnt.Error.HasValue)
    {
        isRunning = false;
        Console.WriteLine(evnt.Error.Value.ResponseHeader);
    }

    switch (evnt.ReplyType)
    {
        case XEventType.Expose:
            draw_interface();
            break;

        case XEventType.KeyPress:
            {
                var keyPressEvent = evnt.As<KeyPressEvent>();
                if (keyPressEvent is { Detail: 45, State: KeyButMask.Control })
                {
                    Console.WriteLine("*** GRABBED KEY: Ctrl+K detected! ***");
                    connection.AllowEventsUnchecked(EventsMode.SyncKeyboard, keyPressEvent.TimeStamp);
                    break;
                }

                switch (keyPressEvent.Detail)
                {
                    case 10: demo_change_hosts(); break; // 1
                    case 11: demo_keyboard_control(); break; // 2
                    case 12: demo_grab_button(); break; // 3
                    case 13: demo_grab_key(); break; // 4
                    case 14: demo_store_color(); break; // 5
                    case 15: demo_ungrab_all(); break; // 6
                    case 43: show_help(); break; // h
                    case 24:
                        {
                            demo_ungrab_all();
                            connection.FreeGCUnchecked(gc);
                            connection.KillClientUnchecked(window);
                            break;
                        }
                }

                break;
            }

        case XEventType.ButtonPress:
            {
                var bp = evnt.As<ButtonPressEvent>();
                if (bp is { Detail: Button.RightButton, State: KeyButMask.Control })
                {
                    Console.WriteLine("*** GRABBED BUTTON: Ctrl+Right Click detected! ***");
                    connection.AllowEventsUnchecked(EventsMode.SyncPointer, bp.TimeStamp);
                }

                break;
            }
    }
}

return 0;

void demo_change_hosts()
{
    Console.WriteLine("=== ChangeHosts Demo ===\n");
    var hosts = connection.ListHosts();
    foreach (var s in hosts.Value.Hosts)
    {
        Console.WriteLine("ablaible hosts: " + s);
    }

    connection.ChangeHostsUnchecked(HostMode.Insert,
        Family.Internet, [127, 0, 0, 1]);

    Console.WriteLine("ChangeHosts: Successfully added localhost to access list\n");
}

void demo_keyboard_control()
{
    Console.WriteLine("=== ChangeKeyboardControl Demo ===\n");
    connection.ChangeKeyboardControlUnchecked(
        KeyboardControlMask.KeyClickPercent | KeyboardControlMask.BellPercent | KeyboardControlMask.BellPitch,
        [80, 90, 1200]);
    Console.WriteLine("Keyboard: Click 80%%, Bell 90%%, Pitch 1200Hz\n");
}

void demo_grab_button()
{
    Console.WriteLine("=== GrabButton Demo ===\n");
    connection.GrabButtonUnchecked(false, window,
        4 | 8,
        GrabMode.Synchronous, GrabMode.Asynchronous, 0, 0,
        Button.MiddleButton, ModifierMask.Control);

    Console.WriteLine("GrabButton: Ctrl+Right Click grabbed! Try it in the window.\n");
    demoActive = true;
}

void demo_grab_key()
{
    Console.WriteLine("=== GrabKey Demo ===\n");
    connection.GrabKeyUnchecked(false, window,
        ModifierMask.Control, 45,
        GrabMode.Asynchronous, GrabMode.Asynchronous);
    Console.WriteLine("GrabKey: Ctrl+K grabbed! Try pressing Ctrl+K.\n");
}

void demo_store_color()
{
    Console.WriteLine("=== StoreNamedColor Demo ===\n");
    var cookie = connection.AllocColor(colormap, 65535, 0, 0);
    Console.WriteLine("StoreNamedColor: Red color allocated, Pixel value: %u\n", cookie.Value.Pixel);
    connection.ChangeGCUnchecked(gc, GCMask.Foreground, [cookie.Value.Pixel]);
    connection.PolyFillRectangleUnchecked(window, gc, [new Rectangle() { X = 300, Y = 50, Width = 80, Height = 30 }]);
}

void demo_ungrab_all()
{
    Console.WriteLine("=== Ungrab Demo ===\n");
    connection.UngrabButtonUnchecked(Button.MiddleButton, window, ModifierMask.Control);
    connection.UngrabKeyUnchecked(45, window, ModifierMask.Control);
    Console.WriteLine("UngrabButton & UngrabKey: All grabs released");
    demoActive = false;
}

void show_help()
{
    Console.WriteLine("=== Available Demos ===");
    Console.WriteLine("1 - ChangeHosts (add localhost)");
    Console.WriteLine("2 - ChangeKeyboardControl (modify keyboard settings)");
    Console.WriteLine("3 - GrabButton (grab Ctrl+Right Click)");
    Console.WriteLine("4 - GrabKey (grab Ctrl+K)");
    Console.WriteLine("5 - StoreNamedColor (allocate red color)");
    Console.WriteLine("6 - UngrabButton/UngrabKey (release all grabs)");
    Console.WriteLine("h - Show this help");
    Console.WriteLine("q - Quit");
}

void draw_interface()
{
    connection.ChangeGCUnchecked(gc, GCMask.Foreground, [screen.WhitePixel]);
    connection.PolyFillRectangleUnchecked(window, gc, [
        new Rectangle() { X = 0, Y = 0, Width = 500, Height = 400 }
    ]);
    connection.ChangeGCUnchecked(gc, GCMask.Foreground, [screen.BlackPixel]);
    connection.ImageText8Unchecked(window, gc, 20, 30, "XCB System Control Demo"u8);
    connection.ImageText8Unchecked(window, gc, 20, 60, "Press number keys (1-6) for demos"u8);
    connection.ImageText8Unchecked(window, gc, 20, 90, "Press 'h' for help, 'q' to quit"u8);

    if (demoActive)
    {
        connection.ImageText8Unchecked(window, gc, 20, 120, "Demo active! Try the grabbed controls."u8);
    }
}