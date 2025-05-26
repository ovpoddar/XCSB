using System.Buffers;
using System.Text;
using Xcsb;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Event;

const int WIDTH = 50;
const int HEIGHT = 50;

var xcsb = XcsbClient.Initialized();
var window = xcsb.NewId();
var screen = xcsb.HandshakeSuccessResponseBody.Screens[0];
xcsb.CreateWindow(
    window,
    screen.Root,
    0, 0, WIDTH, HEIGHT,
    10, ClassType.InputOutput,
    screen.RootVisualId,
    ValueMask.BackgroundPixel | ValueMask.EventMask,
    [screen.WhitePixel, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask)]
);

xcsb.ChangeProperty(PropertyMode.Replace, window, 39, 31, Encoding.UTF8.GetBytes("working fixing dodo"));

var gc = xcsb.NewId();
xcsb.CreateGC(gc, window, GCMask.Foreground | GCMask.GraphicsExposures, [screen.BlackPixel, 0]);
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

Span<byte> buffer = stackalloc byte[XcsbClient.GetEventSize()];
var isRunning = true;

while (isRunning)
{
    ref var evnt = ref xcsb.GetEvent(buffer);
    if (evnt.EventType == EventType.Error)
    {
        Console.WriteLine(evnt.ErrorEvent.ErrorCode.ToString());
        isRunning = false;
    }
    if (evnt.EventType == EventType.Expose)
    {
        xcsb.PutImage(ImageFormat.ZPixmap,
            window,
            gc,
            WIDTH,
            HEIGHT,
            0, 0, 0,
            screen.RootDepth!.DepthValue,
            data.AsSpan()[..requirByte]);
    }
    buffer.Clear();
}