using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb;
using Xcsb.Connection;
using Xcsb.Connection.Models.Handshake;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.BigRequests;
using Xcsb.Extension.Damage;
using Xcsb.Extension.Damage.Models;
using Xcsb.Extension.Damage.Response.Events;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Response.Event;
using Xcsb.SockAccesser;
using static System.Net.Mime.MediaTypeNames;

const int WIDTH = 500;
const int HEIGHT = 500;
const int FACTOR = 4;

using var c = XcsbClient.Connect();
if (c.HandshakeSuccessResponseBody == null)
    return;

var s = c.Extensation.Damage();
if (s is null) return;

var window = c.NewId();
var gc = c.NewId();
var d = c.Initialized();
d.CreateWindowChecked(c.HandshakeSuccessResponseBody.Screens[0].RootDepth.DepthValue, window, c.HandshakeSuccessResponseBody.Screens[0].Root,
    0, 0, WIDTH, HEIGHT, 10, Xcsb.Models.ClassType.InputOutput, c.HandshakeSuccessResponseBody.Screens[0].RootVisualId,
    Xcsb.Masks.ValueMask.BackgroundPixel | Xcsb.Masks.ValueMask.EventMask,
    [c.HandshakeSuccessResponseBody.Screens[0].BlackPixel, (uint)EventMask.ExposureMask]);

d.CreateGCChecked(gc, window, GCMask.Foreground | GCMask.GraphicsExposures,
    [c.HandshakeSuccessResponseBody.Screens[0].BlackPixel, 0]);

d.MapWindowChecked(window);
var body = new byte[WIDTH * HEIGHT * 4];
for (var y = 0; y < HEIGHT; y++)
{
    for (var x = 0; x < WIDTH; x++)
    {
        var index = (y * WIDTH + x) * 4;
        body[index + 0] = (byte)(((x + y) * 255) / (WIDTH + HEIGHT));
        body[index + 1] = (byte)(y * 255 / HEIGHT);
        body[index + 2] = (byte)(x * 255 / WIDTH);
        body[index + 3] = 0;
    }
}

var w = WIDTH / FACTOR;
var h = HEIGHT / FACTOR;
var body1 = new byte[w * h * 4];
for (var y = 0; y < h; y++)
{
    for (var x = 0; x < w; x++)
    {
        var index = (y * w + x) * 4;
        body1[index + 0] = (byte)(((x + y) * 255) / w + h);
        body1[index + 1] = (byte)(y * 255 / w);
        body1[index + 2] = (byte)(x * 255 / h);
        body1[index + 3] = 0;
    }
}

while (true)
{
    var evnt = d.GetEvent();
    if (evnt.ReplyType == Xcsb.Models.XEventType.LastEvent || evnt.Error.HasValue)
    { break; }

    if (evnt.ReplyType == Xcsb.Models.XEventType.Expose)
    {
        d.PutImageChecked(ImageFormatBitmap.ZPixmap,
            window,
            gc,
            WIDTH,
            HEIGHT,
            0,
            0,
            0,
            c.HandshakeSuccessResponseBody.Screens[0].RootDepth.DepthValue,
            body);
        d.PutImageChecked(ImageFormatBitmap.ZPixmap,
            window,
            gc,
            (ushort)w,
            (ushort)h,
            0, 0,
            0,
            c.HandshakeSuccessResponseBody.Screens[0].RootDepth.DepthValue,
            body1
            );
    }
    if (evnt.ReplyType == XEventType.Unknown)
    {
        var damag = evnt.As<GenericEvent>().As<DamageNotifyEvent>();
        Console.WriteLine($"{damag.ResponseHeader} {damag.Drawable} {damag.Damage} {damag.Timestamp} {damag.Area} {damag.Geometry}");

    }
}
