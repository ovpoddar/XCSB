using System.Buffers;
using System.Text;
using Xcsb;
using Xcsb.Event;
using Xcsb.Masks;
using Xcsb.Models;

const int WIDTH = 50;
const int HEIGHT = 50;

var xcsb = XcsbClient.Initialized();
var window = xcsb.NewId();
var screen = xcsb.HandshakeSuccessResponseBody.Screens[0];
var extensations = xcsb.ListExtensions();
Console.Write("available extensions: ");
foreach (var extensation in extensations.Names)
    Console.WriteLine($"    {extensation}");

var extension = xcsb.QueryExtension( Encoding.UTF8.GetBytes(extensations.Names[5]));
Console.WriteLine(extension.FirstEvent);

var rootProprityes = xcsb.ListProperties(screen.Root);
Console.Write("root properties: ");
foreach (var atom in rootProprityes.Atoms)
{
    var atomName = xcsb.GetAtomName(atom);
    Console.WriteLine(atomName.Name);
}

xcsb.CreateWindow(screen.RootDepth.DepthValue,
    window,
    screen.Root,
    0, 0, WIDTH * 10, HEIGHT * 10,
    10, ClassType.InputOutput,
    screen.RootVisualId,
    ValueMask.BackgroundPixel | ValueMask.EventMask,
    [screen.WhitePixel, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask)]
);

xcsb.ChangeProperty(PropertyMode.Replace, window, 39, 31, Encoding.UTF8.GetBytes("working fixing dodo"));

var gc = xcsb.NewId();
xcsb.CreateGC(gc, window, GCMask.Foreground | GCMask.GraphicsExposures, [screen.BlackPixel, 0]);

var white_gc = xcsb.NewId();
xcsb.CreateGC(white_gc, window, GCMask.Foreground | GCMask.GraphicsExposures, [screen.WhitePixel, 0]);

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

xcsb.MapWindow(window);

var isRunning = true;

while (isRunning)
{
    var evnt = xcsb.GetEvent();
    if (!evnt.HasValue) return;
    if (evnt.Value.EventType == EventType.Error)
    {
        Console.WriteLine(evnt.Value.ErrorEvent.ErrorCode.ToString());
        isRunning = false;
    }
    if (evnt.Value.EventType == EventType.Expose)
    {
        xcsb.PutImage(ImageFormatBitmap.ZPixmap,
            window,
            gc,
            WIDTH,
            HEIGHT,
            300, 0, 0,
            screen.RootDepth!.DepthValue,
            data.AsSpan()[..requirByte]);

        xcsb.PolyRectangle(window, gc, [new Rectangle { X = 5, Y = 10, Width = 80, Height = 50 },
            new Rectangle { X = 150, Y = 10, Width = 80, Height = 50 }]);
        xcsb.PolyFillRectangle(window, gc, [new Rectangle { X = 5, Y = 80, Width = 80, Height = 50 },
            new Rectangle { X = 150, Y = 80, Width = 80, Height = 50 }]);

        xcsb.FillPoly(window, gc, PolyShape.Convex, CoordinateMode.Origin, [new() { X = 120, Y = 130 }, new() { X = 80, Y = 180 }, new() { X = 160, Y = 180 }]);

        xcsb.PolyArc(window, gc, [new Arc {X =  20, Y = 200, Width = 40, Height = 40,Angle1 =  0, Angle2 = 360 * 64},
                    new Arc { X = 100, Y = 200, Width = 30,Height = 30,Angle1 = 0, Angle2 = 180 * 64},
                    new Arc { X = 180, Y = 200, Width = 35,Height = 25,Angle1 = 45 * 64, Angle2 =90 * 64}]);
        xcsb.PolyFillArc(window, gc, [new Arc {X =  20, Y = 250, Width = 40, Height = 40,Angle1 =  0, Angle2 = 360 * 64},
                    new Arc { X = 100, Y = 250, Width = 30,Height = 30,Angle1 = 0, Angle2 = 180 * 64},
                    new Arc { X = 180, Y = 250, Width = 35,Height = 25,Angle1 = 45 * 64, Angle2 =90 * 64}]);

        xcsb.PolyLine(CoordinateMode.Origin, window, gc, [new Point { X = 10, Y = 300 }, new Point { X = 180, Y = 300 }]);
        xcsb.PolyPoint(CoordinateMode.Origin, window, gc, [new Point { X = 10, Y = 305 }, new Point { X = 180, Y = 305 }]);

        xcsb.PolySegment(window, gc, [
            new Segment{ X1 = 90, Y1 = 55, X2= 100,Y2= 65},
            new Segment{ X1 = 100, Y1 = 55, X2= 90,Y2= 65}]);

        xcsb.CopyArea(window, window, gc,
            300, 0, 300, HEIGHT + 10, WIDTH, HEIGHT);

        xcsb.CopyPlane(window, window, white_gc,
            300, 0, 300, (HEIGHT * 2) + 10, WIDTH, HEIGHT, 4);
    }
}