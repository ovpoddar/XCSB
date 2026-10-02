using Xcsb;
using Xcsb.Connection;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.TypeInfo;
using Xcsb.Response.Event;

using var connection = XcsbClient.Connect();
var c = connection.Initialized();

var window = connection.NewId();
c.CreateWindowChecked(0,
    window,
    connection.HandshakeSuccessResponseBody.Screens[0].Root,
 0,
 0,
 500,
 500,
 0,
 ClassType.InputOutput,
 connection.HandshakeSuccessResponseBody.Screens[0].RootVisualId,
 ValueMask.BackgroundPixel | ValueMask.EventMask,
 [0x00ffffff, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask)]
);

c.MapWindowChecked(window);
var isRunning = true;

var fontId = connection.NewId();
var fontId1 = connection.NewId();
var isExecuted = false;

while (isRunning)
{
    using var Event = c.GetEvent();

    if (Event.Reply.ReplyType == EventType.LastEvent) return;
    if (Event.Reply.Error.HasValue)
    {
        Console.WriteLine(Event.Reply.Error.Value.Message);
        isRunning = false;
        break;
    }
    else if (Event.Reply.ReplyType == EventType.KeyPress)
    {
        if (!isExecuted)
        {
            c.ChangeWindowAttributesChecked(window,
                ValueMask.BackgroundPixel | ValueMask.EventMask,
                [0x00ffffff, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask | EventMask.ButtonPressMask)]);
            isExecuted = true;
        }

        ref readonly KeyPressEvent keyPressEvent = ref Event.Reply.As<KeyPressEvent>();
        if (keyPressEvent.Detail == 24)//d
        {
            c.DestroyWindowChecked(window);
            isRunning = false;
        }
        if (keyPressEvent.Detail == 46) //c
        {
            c.CirculateWindowChecked(Circulate.LowerHighest, window);
        }

        if (keyPressEvent.Detail == 58) //m
        {
            c.UnmapWindowChecked(window);
            Thread.Sleep(1000);
            c.MapWindowChecked(window);
        }

        if (keyPressEvent.Detail == 25)// w
        {

            c.OpenFontChecked("-misc-fixed-*-*-*-*-13-*-*-*-*-*-iso10646-1", fontId);
            c.OpenFontChecked("fixed", fontId1);

            var gc = connection.NewId();
            c.CreateGcChecked(gc, window, GcMask.Foreground | GcMask.Background | GcMask.Font, [connection.HandshakeSuccessResponseBody.Screens[0].BlackPixel, connection.HandshakeSuccessResponseBody.Screens[0].WhitePixel, fontId]);
            var gc1 = connection.NewId();
            c.CreateGcChecked(gc1, window, GcMask.Foreground | GcMask.Background | GcMask.Font, [connection.HandshakeSuccessResponseBody.Screens[0].BlackPixel, connection.HandshakeSuccessResponseBody.Screens[0].WhitePixel, fontId1]);

            c.ImageText16Checked(window, gc, 10, 15, "this is a utf 16 string");
            c.ImageText8Checked(window, gc, 10, 40, "this is a utf 8 string"u8);
            c.PolyText8Checked(window, gc, 10, 80, ["Helloworld with polytext"u8, "polytext2"u8]);
            c.PolyText16Checked(window, gc, 10, 100, ["Hello world"]);
            c.FreeGcChecked(gc);
            c.FreeGcChecked(gc1);

            c.CloseFontChecked(fontId1);
            c.CloseFontChecked(fontId);
        }

        if (keyPressEvent.Detail == 54) //c
        {
            var gc = connection.NewId();
            c.CreateGcChecked(gc, window, GcMask.Foreground, [0x00ffffff]);

            c.PolyFillRectangleChecked(window, gc, [new Rectangle{
                X = 0,Y= 0,Width =  500, Height = 500
            }]);

            c.FreeGcChecked(gc);
        }

        c.BellChecked(100);
        Console.WriteLine($"event {Event.Reply.ReplyType} {keyPressEvent.Detail}");
    }
    else if (Event.Reply.ReplyType == EventType.Expose)
    {
        var gc = connection.NewId();
        c.CreateGcChecked(gc, window, GcMask.Foreground, [0x00ff0000]);

        c.PolyFillRectangleChecked(window, gc, [new Rectangle{
                X = 0,Y= 0,Width =  500, Height = 500
            }]);

        c.FreeGcChecked(gc);
    }
    else if (Event.Reply.ReplyType == EventType.ButtonPress)
    {
        if (Event.Reply.As<ButtonPressEvent>().Detail == Button.LeftButton)
        {
            using var currentPos = c.QueryPointer(connection.HandshakeSuccessResponseBody.Screens[0].Root);
            Console.WriteLine($"before warp the pointer {currentPos.Reply.RootX}   {currentPos.Reply.RootY}");
            c.WarpPointerChecked(0, window, 0, 0, 0, 0, 200, 150);
            using var currentPos1 = c.QueryPointer(connection.HandshakeSuccessResponseBody.Screens[0].Root);
            Console.WriteLine($"before warp the pointer {currentPos1.Reply.RootX}   {currentPos1.Reply.RootY}");
        }

    }
    else
    {
        Console.WriteLine(Event.Reply.ReplyType);
    }
}