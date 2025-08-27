using Xcsb;
using Xcsb.Event;
using Xcsb.Masks;
using Xcsb.Models;

const int width = 400;
const int height = 300;

var xcsb = XcsbClient.Initialized();
var screen = xcsb.HandshakeSuccessResponseBody.Screens[0];
var window = xcsb.NewId();

xcsb.CreateWindow(0,
    window,
    screen.Root,
    100, 100, width, height,
    2, ClassType.InputOutput,
    screen.RootVisualId,
    ValueMask.BackgroundPixel | ValueMask.EventMask,
    [screen.WhitePixel, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask)]
);

var pixmap = xcsb.NewId();
xcsb.CreatePixmap(screen.RootDepth!.DepthValue,
    pixmap,
    screen.Root,
    width, height);

var gc = xcsb.NewId();
xcsb.CreateGC(gc, pixmap, GCMask.Foreground | GCMask.Background, [screen.BlackPixel, screen.WhitePixel]);

var cursor = xcsb.NewId();

var cursor_pixmap = xcsb.NewId();
var cursor_mask = xcsb.NewId();

xcsb.CreatePixmap(1, cursor_pixmap, screen.Root, 16, 16);
xcsb.CreatePixmap(1, cursor_mask, screen.Root, 16, 16);

var cursor_gc = xcsb.NewId();
xcsb.CreateGC(cursor_gc, cursor_pixmap, GCMask.Foreground, [1]);

xcsb.PolySegment(cursor_pixmap, cursor_gc,
[
    new Segment { X1 = 8, Y1 = 0, X2 = 8, Y2 = 15 },
    new Segment { X1 = 0, Y1 = 8, X2 = 15, Y2 = 8 }
]);
xcsb.PolySegment(cursor_mask, cursor_gc,
[
    new Segment { X1 = 8, Y1 = 0, X2 = 8, Y2 = 15 },
    new Segment { X1 = 0, Y1 = 8, X2 = 15, Y2 = 8 }
]);

xcsb.CreateCursor(cursor, cursor_pixmap, cursor_mask,
    0, 0, 0,
    65535, 65535, 65535,
    8, 8);

xcsb.RecolorCursor(cursor,
    65535, 0, 0,
    0, 0, 65535);

xcsb.ChangeWindowAttributes(window, ValueMask.Cursor, [cursor]);

xcsb.ChangeGC(gc, GCMask.Foreground, [screen.WhitePixel]);
xcsb.PolyFillRectangle(pixmap, gc, [new Rectangle { X = 0, Y = 0, Width = width, Height = height }]);

xcsb.SetClipRectangles(ClipOrdering.Unsorted,
    gc,
    0, 0,
    [
        new Rectangle { X = 50, Y = 50, Width = 100, Height = 80 },
        new Rectangle { X = 200, Y = 100, Width = 120, Height = 60 },
        new Rectangle { X = 100, Y = 180, Width = 80, Height = 90 }
    ]);

xcsb.SetDashes(
    gc,
    0,
    [10, 5, 3, 7]);

xcsb.ChangeGC(gc, GCMask.LineStyle, [1]);

xcsb.ChangeGC(gc, GCMask.Foreground, [screen.BlackPixel]);

xcsb.PolyFillRectangle(pixmap, gc, [new Rectangle { X = 0, Y = 0, Width = width, Height = height }]);

xcsb.PolyRectangle(pixmap, gc, [new Rectangle { X = 5, Y = 5, Width = width - 10, Height = height - 10 }]);

xcsb.ChangeGC(gc, GCMask.ClipMask, [0]);

xcsb.PolyRectangle(pixmap, gc, [
    new Rectangle { X = 50, Y = 50, Width = 100, Height = 80 },
    new Rectangle { X = 200, Y = 100, Width = 120, Height = 60 },
    new Rectangle { X = 100, Y = 180, Width = 80, Height = 90 }
]);

xcsb.MapWindow(window);

xcsb.FreeCursor(cursor);
xcsb.FreePixmap(cursor_pixmap);
xcsb.FreePixmap(cursor_mask);
xcsb.FreeGC(cursor_gc);

var isRunning = true;

var windowDetails = xcsb.GetWindowAttributes(window);
if (windowDetails.Class != ClassResponseType.InputOutput)
    return;
var windowGeometry = xcsb.GetGeometry(window);
if (windowGeometry is { X: 100, Y: 100 })
    xcsb.ConfigureWindowChecked(window,
        ConfigureValueMask.X | ConfigureValueMask.Y,
        [500, 500]);

var query = xcsb.QueryTree(window);
if (query.Root != xcsb.HandshakeSuccessResponseBody.Screens[0].Root)
    return;


while (isRunning)
{
    var evnt = xcsb.GetEvent();
    if (!evnt.HasValue) return;
    switch (evnt.Value.Reply)
    {
        case EventType.Expose:
        {
            xcsb.CopyArea(pixmap,
                window,
                gc,
                0, 0,
                0, 0,
                width, height);
            break;
        }

        case EventType.KeyPress:
            isRunning = false;
            break;
    }
}

xcsb.FreePixmap(pixmap);
xcsb.FreeGC(gc);
xcsb.DestroyWindow(window);