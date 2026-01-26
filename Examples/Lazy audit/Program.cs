using System.Buffers;
using System.Text;
using Xcsb;
using Xcsb.Extension.Generic.Event;
using Xcsb.Extension.Generic.Event.Masks;
using Xcsb.Extension.Generic.Event.Models;

const int WIDTH = 50;
const int HEIGHT = 50;
using var connection = XcsbClient.Connect();
var xcsb = connection.Initialized();
var window = connection.NewId();
var screen = connection.HandshakeSuccessResponseBody.Screens[0];
var lazyXcsb = xcsb.BufferClient;

xcsb.CreateWindowUnchecked(screen.RootDepth.DepthValue,
    window,
    screen.Root,
    0, 0, WIDTH * 10, HEIGHT * 10,
    10, ClassType.InputOutput,
    screen.RootVisualId,
    ValueMask.BackgroundPixel | ValueMask.EventMask,
    [screen.WhitePixel, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask)]
);

lazyXcsb.ChangeProperty<byte>(PropertyMode.Replace, window, ATOM.WmName, ATOM.String, Encoding.UTF8.GetBytes("working fixing dodo"));

var gc = connection.NewId();
lazyXcsb.CreateGC(gc, window, GCMask.Foreground | GCMask.GraphicsExposures, [screen.BlackPixel, 0]);

var white_gc = connection.NewId();
lazyXcsb.CreateGC(white_gc, window, GCMask.Foreground | GCMask.GraphicsExposures, [screen.WhitePixel, 0]);

var requirByte = WIDTH * HEIGHT * 4;
var data = ArrayPool<byte>.Shared.Rent(requirByte);

for (var y = 0; y < HEIGHT; y++)
{
    for (var x = 0; x < WIDTH; x++)
    {
        var index = (y * WIDTH + x) * 4;
        data[index + 0] = (byte)(((x + y) * 255) / (WIDTH + HEIGHT));
        data[index + 1] = (byte)(y * 255 / HEIGHT);
        data[index + 2] = (byte)(x * 255 / WIDTH);
        data[index + 3] = 0;
    }
}

lazyXcsb.MapWindow(window);
lazyXcsb.Flush();

var isRunning = true;

while (isRunning)
{
    var evnt = xcsb.GetEvent();
    if (evnt.ReplyType == XEventType.LastEvent) return;
    if (evnt.ReplyType == XEventType.Expose)
    {
        lazyXcsb.PutImage(ImageFormatBitmap.ZPixmap,
            window,
            gc,
            WIDTH,
            HEIGHT,
            300, 0, 0,
            screen.RootDepth!.DepthValue,
            data.AsSpan()[..requirByte]);

        lazyXcsb.PolyRectangle(window, gc, [new Rectangle { X = 5, Y = 10, Width = 80, Height = 50 },
            new Rectangle { X = 150, Y = 10, Width = 80, Height = 50 }]);
        lazyXcsb.PolyFillRectangle(window, gc, [new Rectangle { X = 5, Y = 80, Width = 80, Height = 50 },
            new Rectangle { X = 150, Y = 80, Width = 80, Height = 50 }]);

        lazyXcsb.FillPoly(window, gc, PolyShape.Convex, CoordinateMode.Origin, [new() { X = 120, Y = 130 }, new() { X = 80, Y = 180 }, new() { X = 160, Y = 180 }]);

        lazyXcsb.PolyArc(window, gc, [new Arc {X =  20, Y = 200, Width = 40, Height = 40,Angle1 =  0, Angle2 = 360 * 64},
                    new Arc { X = 100, Y = 200, Width = 30,Height = 30,Angle1 = 0, Angle2 = 180 * 64},
                    new Arc { X = 180, Y = 200, Width = 35,Height = 25,Angle1 = 45 * 64, Angle2 =90 * 64}]);
        lazyXcsb.PolyFillArc(window, gc, [new Arc {X =  20, Y = 250, Width = 40, Height = 40,Angle1 =  0, Angle2 = 360 * 64},
                    new Arc { X = 100, Y = 250, Width = 30,Height = 30,Angle1 = 0, Angle2 = 180 * 64},
                    new Arc { X = 180, Y = 250, Width = 35,Height = 25,Angle1 = 45 * 64, Angle2 =90 * 64}]);

        lazyXcsb.PolyLine(CoordinateMode.Origin, window, gc, [new Point { X = 10, Y = 300 }, new Point { X = 180, Y = 300 }]);
        lazyXcsb.PolyPoint(CoordinateMode.Origin, window, gc, [new Point { X = 10, Y = 305 }, new Point { X = 180, Y = 305 }]);

        lazyXcsb.PolySegment(window, gc, [
            new Segment{ X1 = 90, Y1 = 55, X2= 100,Y2= 65},
            new Segment{ X1 = 100, Y1 = 55, X2= 90,Y2= 65}]);

        lazyXcsb.CopyArea(window, window, gc,
            300, 0, 300, HEIGHT + 10, WIDTH, HEIGHT);

        lazyXcsb.CopyPlane(window, window, white_gc,
            300, 0, 300, (HEIGHT * 2) + 10, WIDTH, HEIGHT, 4);
        lazyXcsb.Flush();
    }
}