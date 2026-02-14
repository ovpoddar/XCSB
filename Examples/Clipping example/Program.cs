using Xcsb;
using Xcsb.Connection;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Response.Event;

const int width = 400;
const int height = 300;

using var connection = XcsbClient.Connect();
var xcsb = connection.Initialized();
var screen = connection.HandshakeSuccessResponseBody.Screens[0];
var window = connection.NewId();

xcsb.CreateWindowUnchecked(0,
    window,
    screen.Root,
    100, 100, width, height,
    2, ClassType.InputOutput,
    screen.RootVisualId,
    ValueMask.BackgroundPixel | ValueMask.EventMask,
    [screen.WhitePixel, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask)]
);

var pixmap = connection.NewId();
xcsb.CreatePixmapUnchecked(screen.RootDepth!.DepthValue,
    pixmap,
    screen.Root,
    width, height);

var gc = connection.NewId();
xcsb.CreateGCUnchecked(gc, pixmap, GcMask.Foreground | GcMask.Background, [screen.BlackPixel, screen.WhitePixel]);

var cursor = connection.NewId();

var cursor_pixmap = connection.NewId();
var cursor_mask = connection.NewId();

xcsb.CreatePixmapUnchecked(1, cursor_pixmap, screen.Root, 16, 16);
xcsb.CreatePixmapUnchecked(1, cursor_mask, screen.Root, 16, 16);

var cursor_gc = connection.NewId();
xcsb.CreateGCUnchecked(cursor_gc, cursor_pixmap, GcMask.Foreground, [1]);

xcsb.PolySegmentUnchecked(cursor_pixmap, cursor_gc,
[
    new Segment { X1 = 8, Y1 = 0, X2 = 8, Y2 = 15 },
    new Segment { X1 = 0, Y1 = 8, X2 = 15, Y2 = 8 }
]);
xcsb.PolySegmentUnchecked(cursor_mask, cursor_gc,
[
    new Segment { X1 = 8, Y1 = 0, X2 = 8, Y2 = 15 },
    new Segment { X1 = 0, Y1 = 8, X2 = 15, Y2 = 8 }
]);

xcsb.CreateCursorUnchecked(cursor, cursor_pixmap, cursor_mask,
    0, 0, 0,
    65535, 65535, 65535,
    8, 8);

xcsb.RecolorCursorUnchecked(cursor,
    65535, 0, 0,
    0, 0, 65535);

xcsb.ChangeWindowAttributesUnchecked(window, ValueMask.Cursor, [cursor]);

xcsb.ChangeGCUnchecked(gc, GcMask.Foreground, [screen.WhitePixel]);
xcsb.PolyFillRectangleUnchecked(pixmap, gc, [new Rectangle { X = 0, Y = 0, Width = width, Height = height }]);

xcsb.SetClipRectanglesUnchecked(ClipOrdering.Unsorted,
    gc,
    0, 0,
    [
        new Rectangle { X = 50, Y = 50, Width = 100, Height = 80 },
        new Rectangle { X = 200, Y = 100, Width = 120, Height = 60 },
        new Rectangle { X = 100, Y = 180, Width = 80, Height = 90 }
    ]);

xcsb.SetDashesUnchecked(
    gc,
    0,
    [10, 5, 3, 7]);

xcsb.ChangeGCUnchecked(gc, GcMask.LineStyle, [1]);

xcsb.ChangeGCUnchecked(gc, GcMask.Foreground, [screen.BlackPixel]);

xcsb.PolyFillRectangleUnchecked(pixmap, gc, [new Rectangle { X = 0, Y = 0, Width = width, Height = height }]);

xcsb.PolyRectangleUnchecked(pixmap, gc, [new Rectangle { X = 5, Y = 5, Width = width - 10, Height = height - 10 }]);

xcsb.ChangeGCUnchecked(gc, GcMask.ClipMask, [0]);

xcsb.PolyRectangleUnchecked(pixmap, gc, [
    new Rectangle { X = 50, Y = 50, Width = 100, Height = 80 },
    new Rectangle { X = 200, Y = 100, Width = 120, Height = 60 },
    new Rectangle { X = 100, Y = 180, Width = 80, Height = 90 }
]);

xcsb.MapWindowUnchecked(window);

xcsb.FreeCursorUnchecked(cursor);
xcsb.FreePixmapUnchecked(cursor_pixmap);
xcsb.FreePixmapUnchecked(cursor_mask);
xcsb.FreeGCUnchecked(cursor_gc);

var isRunning = true;

var windowDetails = xcsb.GetWindowAttributes(window);
if (windowDetails.Class != ClassResponseType.InputOutput)
    return;
var windowGeometry = xcsb.GetGeometry(window);
if (windowGeometry is { X: 38, Y: 59 })
    xcsb.ConfigureWindowChecked(window,
        ConfigureValueMask.X | ConfigureValueMask.Y,
        [500, 500]);

var query = xcsb.QueryTree(window);
if (query.Root != connection.HandshakeSuccessResponseBody.Screens[0].Root)
    return;


while (isRunning)
{
    var evnt = xcsb.GetEvent();
    if (evnt.ReplyType == XEventType.LastEvent) return;
    switch (evnt.ReplyType)
    {
        case XEventType.Expose:
            {
                xcsb.CopyAreaUnchecked(pixmap,
                    window,
                    gc,
                    0, 0,
                    0, 0,
                    width, height);
                break;
            }

        case XEventType.KeyPress:
            isRunning = false;
            break;
    }
}

xcsb.FreePixmapUnchecked(pixmap);
xcsb.FreeGCUnchecked(gc);
xcsb.DestroyWindowUnchecked(window);