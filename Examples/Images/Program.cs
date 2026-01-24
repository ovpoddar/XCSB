using System.Buffers;
using System.Text;
using Xcsb;
using Xcsb.Extension.Generic.Event;
using Xcsb.Extension.Generic.Event.Masks;
using Xcsb.Models;

const int WIDTH = 50;
const int HEIGHT = 50;
using var connection = XcsbClient.Connect();
var xcsb = connection.Initialized();
var window = connection.NewId();
var screen = connection.HandshakeSuccessResponseBody.Screens[0];
var extensations = xcsb.ListExtensions();
Console.Write("available extensions: ");
foreach (var extensation in extensations.Names)
    Console.WriteLine($"    {extensation}");

var extension = xcsb.QueryExtension(Encoding.UTF8.GetBytes(extensations.Names[5]));
Console.WriteLine(extension.FirstEvent);

var rootProprityes = xcsb.ListProperties(screen.Root);
Console.Write("root properties: ");
foreach (var atom in rootProprityes.Atoms)
{
    var atomName = xcsb.GetAtomName(atom);
    Console.WriteLine(atomName.Name);
}

xcsb.CreateWindowUnchecked(screen.RootDepth.DepthValue,
    window,
    screen.Root,
    0, 0, WIDTH * 10, HEIGHT * 10,
    10, ClassType.InputOutput,
    screen.RootVisualId,
    ValueMask.BackgroundPixel | ValueMask.EventMask,
    [screen.WhitePixel, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask)]
);

xcsb.ChangePropertyUnchecked<byte>(PropertyMode.Replace, window, ATOM.WmName, ATOM.String, Encoding.UTF8.GetBytes("working fixing dodo"));

var gc = connection.NewId();
xcsb.CreateGCUnchecked(gc, window, GCMask.Foreground | GCMask.GraphicsExposures, [screen.BlackPixel, 0]);

var white_gc = connection.NewId();
xcsb.CreateGCUnchecked(white_gc, window, GCMask.Foreground | GCMask.GraphicsExposures, [screen.WhitePixel, 0]);

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

xcsb.MapWindowUnchecked(window);

var isRunning = true;

while (isRunning)
{
    var evnt = xcsb.GetEvent();
    if (evnt.ReplyType == XEventType.LastEvent) return;

    if (evnt.Error.HasValue)
    {
        Console.WriteLine(evnt.Error.Value.ResponseHeader.Reply);
        isRunning = false;
    }

    if (evnt.ReplyType == XEventType.Expose)
    {
        xcsb.PutImageUnchecked(ImageFormatBitmap.ZPixmap,
            window,
            gc,
            WIDTH,
            HEIGHT,
            300, 0, 0,
            screen.RootDepth!.DepthValue,
            data.AsSpan()[..requirByte]);


        xcsb.PolyRectangleUnchecked(window, gc, [
            new Rectangle { X = 5, Y = 10, Width = 80, Height = 50 },
            new Rectangle { X = 150, Y = 10, Width = 80, Height = 50 }
        ]);
        xcsb.PolyFillRectangleUnchecked(window, gc, [
            new Rectangle { X = 5, Y = 80, Width = 80, Height = 50 },
            new Rectangle { X = 150, Y = 80, Width = 80, Height = 50 }
        ]);

        xcsb.FillPolyUnchecked(window, gc, PolyShape.Convex, CoordinateMode.Origin,
            [new() { X = 120, Y = 130 }, new() { X = 80, Y = 180 }, new() { X = 160, Y = 180 }]);

        xcsb.PolyArcUnchecked(window, gc, [
            new Arc { X = 20, Y = 200, Width = 40, Height = 40, Angle1 = 0, Angle2 = 360 * 64 },
            new Arc { X = 100, Y = 200, Width = 30, Height = 30, Angle1 = 0, Angle2 = 180 * 64 },
            new Arc { X = 180, Y = 200, Width = 35, Height = 25, Angle1 = 45 * 64, Angle2 = 90 * 64 }
        ]);
        xcsb.PolyFillArcUnchecked(window, gc, [
            new Arc { X = 20, Y = 250, Width = 40, Height = 40, Angle1 = 0, Angle2 = 360 * 64 },
            new Arc { X = 100, Y = 250, Width = 30, Height = 30, Angle1 = 0, Angle2 = 180 * 64 },
            new Arc { X = 180, Y = 250, Width = 35, Height = 25, Angle1 = 45 * 64, Angle2 = 90 * 64 }
        ]);

        xcsb.PolyLineUnchecked(CoordinateMode.Origin, window, gc,
            [new Point { X = 10, Y = 300 }, new Point { X = 180, Y = 300 }]);
        xcsb.PolyPointUnchecked(CoordinateMode.Origin, window, gc,
            [new Point { X = 10, Y = 305 }, new Point { X = 180, Y = 305 }]);

        xcsb.PolySegmentUnchecked(window, gc, [
            new Segment { X1 = 90, Y1 = 55, X2 = 100, Y2 = 65 },
            new Segment { X1 = 100, Y1 = 55, X2 = 90, Y2 = 65 }
        ]);

        xcsb.CopyAreaUnchecked(window, window, gc,
            300, 0, 300, HEIGHT + 10, WIDTH, HEIGHT);

        xcsb.CopyPlaneUnchecked(window, window, white_gc,
            300, 0, 300, (HEIGHT * 2) + 10, WIDTH, HEIGHT, 4);


        var image = xcsb.GetImage(ImageFormat.ZPixmap, window, 300, 0, WIDTH, HEIGHT, uint.MaxValue);
        Console.WriteLine($"First pixels {image.Data[100]} {image.Data[101]} {image.Data[102]} {image.Data[103]}");
    }
}