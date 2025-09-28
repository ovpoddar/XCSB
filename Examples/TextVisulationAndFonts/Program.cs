using System.Text;
using Xcsb;
using Xcsb.Event;
using Xcsb.Masks;
using Xcsb.Models;

using var c = XcsbClient.Initialized();

var window = c.NewId();
c.CreateWindow(0,
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
            c.ChangeWindowAttributes(window,
                ValueMask.BackgroundPixel | ValueMask.EventMask,
                [0x00ffffff, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask | EventMask.ButtonPressMask)]);
            isExecuted = true;
        }

        var keyPressEvent = Event.As<KeyPressEvent>();  
        if (keyPressEvent.Detail == 24)//d
        {
            c.DestroyWindow(window);
            isRunning = false;
        }
        if (keyPressEvent.Detail == 46) //c
        {
            c.CirculateWindow(Circulate.LowerHighest, window);
        }

        if (keyPressEvent.Detail == 58) //m
        {
            c.UnmapWindow(window);
            Thread.Sleep(1000);
            c.MapWindow(window);
        }

        if (keyPressEvent.Detail == 25)// w
        {

            c.OpenFont("-misc-fixed-*-*-*-*-13-*-*-*-*-*-iso10646-1", fontId);
            c.OpenFont("fixed", fontId1);

            var gc = c.NewId();
            c.CreateGC(gc, window, GCMask.Foreground | GCMask.Background | GCMask.Font, [c.HandshakeSuccessResponseBody.Screens[0].BlackPixel, c.HandshakeSuccessResponseBody.Screens[0].WhitePixel, fontId]);
            var gc1 = c.NewId();
            c.CreateGC(gc1, window, GCMask.Foreground | GCMask.Background | GCMask.Font, [c.HandshakeSuccessResponseBody.Screens[0].BlackPixel, c.HandshakeSuccessResponseBody.Screens[0].WhitePixel, fontId1]);

            c.ImageText16(window, gc, 10, 15, "this is a utf 16 string");
            c.ImageText8(window, gc, 10, 40, "this is a utf 8 string"u8);
            var data = Encoding.UTF8.GetBytes("Helloworld");
            c.PolyText8(window, gc, 10, 80, [(byte)data.Length, 0, .. data]);
            c.PolyText16(window, gc, 10, 100, [11, 0,
                0, (byte)'H',
                0, (byte)'e',
                0, (byte)'l',
                0, (byte)'l',
                0, (byte)'o',
                0, (byte)' ',
                0, (byte)'w',
                0, (byte)'o',
                0, (byte)'r',
                0, (byte)'l',
                0, (byte)'d']);
            c.FreeGC(gc);
            c.FreeGC(gc1);

            c.CloseFont(fontId1);
            c.CloseFont(fontId);
        }

        if (keyPressEvent.Detail == 54) //c
        {
            var gc = c.NewId();
            c.CreateGC(gc, window, GCMask.Foreground, [0x00ffffff]);

            c.PolyFillRectangle(window, gc, [new Rectangle{
                X = 0,Y= 0,Width =  500, Height = 500
            }]);

            c.FreeGC(gc);
        }

        c.Bell(100);
        Console.WriteLine($"event {Event.ReplyType} {keyPressEvent.Detail}");
    }
    else if (Event.ReplyType == XEventType.Expose)
    {
        var gc = c.NewId();
        c.CreateGC(gc, window, GCMask.Foreground, [0x00ff0000]);

        c.PolyFillRectangle(window, gc, [new Rectangle{
                X = 0,Y= 0,Width =  500, Height = 500
            }]);

        c.FreeGC(gc);
    }
    else if (Event.ReplyType is XEventType.ButtonPress)
    {
        if (Event.As<ButtonPressEvent>().Detail == Button.LeftButton)
        {
            var currentPos = c.QueryPointer(c.HandshakeSuccessResponseBody.Screens[0].Root);
            Console.WriteLine($"before warp the pointer {currentPos.Value.RootX}   {currentPos.Value.RootY}");
            c.WarpPointer(0, window, 0, 0, 0, 0, 200, 150);
            currentPos = c.QueryPointer(c.HandshakeSuccessResponseBody.Screens[0].Root);
            Console.WriteLine($"before warp the pointer {currentPos.Value.RootX}   {currentPos.Value.RootY}");
        }

    }
    else
    {
        Console.WriteLine(Event.ReplyType);
    }
}