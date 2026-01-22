using System.Text;
using Xcsb;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Response.Event;

using var c = XcsbClient.Initialized();

var window = c.NewId();
c.CheckRequest(c.CreateWindow(0,
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
));

c.CheckRequest(c.MapWindow(window));
var isRunning = true;

var fontId = c.NewId();
var fontId1 = c.NewId();
var isExecuted = false;

while (isRunning)
{
    var Event = c.GetEvent();

    if (Event.ReplyType == XEventType.LastEvent) return;
    if (Event.Error.HasValue)
    {
        Console.WriteLine(Event.Error.Value.ResponseHeader.Reply);
        isRunning = false;
        break;
    }
    else if (Event.ReplyType is XEventType.KeyPress)
    {
        if (!isExecuted)
        {
            c.CheckRequest(c.ChangeWindowAttributes(window,
                ValueMask.BackgroundPixel | ValueMask.EventMask,
                [0x00ffffff, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask | EventMask.ButtonPressMask)]));
            isExecuted = true;
        }

        var keyPressEvent = Event.As<KeyPressEvent>();
        if (keyPressEvent.Detail == 24)//d
        {
            c.CheckRequest(c.DestroyWindow(window));
            isRunning = false;
        }
        if (keyPressEvent.Detail == 46) //c
        {
            c.CheckRequest(c.CirculateWindow(Circulate.LowerHighest, window));
        }

        if (keyPressEvent.Detail == 58) //m
        {
            c.CheckRequest(c.UnmapWindow(window));
            Thread.Sleep(1000);
            c.CheckRequest(c.MapWindow(window));
        }

        if (keyPressEvent.Detail == 25)// w
        {

            c.CheckRequest(c.OpenFont("-misc-fixed-*-*-*-*-13-*-*-*-*-*-iso10646-1", fontId));
            c.CheckRequest(c.OpenFont("fixed", fontId1));

            var gc = c.NewId();
            c.CheckRequest(c.CreateGC(gc, window, GCMask.Foreground | GCMask.Background | GCMask.Font, [c.HandshakeSuccessResponseBody.Screens[0].BlackPixel, c.HandshakeSuccessResponseBody.Screens[0].WhitePixel, fontId]));
            var gc1 = c.NewId();
            c.CheckRequest(c.CreateGC(gc1, window, GCMask.Foreground | GCMask.Background | GCMask.Font, [c.HandshakeSuccessResponseBody.Screens[0].BlackPixel, c.HandshakeSuccessResponseBody.Screens[0].WhitePixel, fontId1]));

            c.CheckRequest(c.ImageText16(window, gc, 10, 15, "this is a utf 16 string"));
            c.CheckRequest(c.ImageText8(window, gc, 10, 40, "this is a utf 8 string"u8));
            c.CheckRequest(c.PolyText8(window, gc, 10, 80, ["Helloworld with polytext"u8, "polytext2"u8]));
            c.CheckRequest(c.PolyText16(window, gc, 10, 100, ["Hello world"]));
            c.CheckRequest(c.FreeGC(gc));
            c.CheckRequest(c.FreeGC(gc1));

            c.CheckRequest(c.CloseFont(fontId1));
            c.CheckRequest(c.CloseFont(fontId));
        }

        if (keyPressEvent.Detail == 54) //c
        {
            var gc = c.NewId();
            c.CheckRequest(c.CreateGC(gc, window, GCMask.Foreground, [0x00ffffff]));

            c.CheckRequest(c.PolyFillRectangle(window, gc, [new Rectangle{
                X = 0,Y= 0,Width =  500, Height = 500
            }]));

            c.CheckRequest(c.FreeGC(gc));
        }

        c.CheckRequest(c.Bell(100));
        Console.WriteLine($"event {Event.ReplyType} {keyPressEvent.Detail}");
    }
    else if (Event.ReplyType == XEventType.Expose)
    {
        var gc = c.NewId();
        c.CheckRequest(c.CreateGC(gc, window, GCMask.Foreground, [0x00ff0000]));

        c.CheckRequest(c.PolyFillRectangle(window, gc, [new Rectangle{
                X = 0,Y= 0,Width =  500, Height = 500
            }]));

        c.CheckRequest(c.FreeGC(gc));
    }
    else if (Event.ReplyType is XEventType.ButtonPress)
    {
        if (Event.As<ButtonPressEvent>().Detail == Button.LeftButton)
        {
            var currentPos = c.QueryPointer(c.HandshakeSuccessResponseBody.Screens[0].Root);
            Console.WriteLine($"before warp the pointer {currentPos.RootX}   {currentPos.RootY}");
            c.CheckRequest(c.WarpPointer(0, window, 0, 0, 0, 0, 200, 150));
            currentPos = c.QueryPointer(c.HandshakeSuccessResponseBody.Screens[0].Root);
            Console.WriteLine($"before warp the pointer {currentPos.RootX}   {currentPos.RootY}");
        }

    }
    else
    {
        Console.WriteLine(Event.ReplyType);
    }
}