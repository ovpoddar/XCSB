using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb;
using Xcsb.Connection;
using Xcsb.Connection.Models.Handshake;
using Xcsb.Extension.BigRequests;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.SockAccesser;

const int WIDTH = 500;
const int HEIGHT = 500;
var data = new byte[250007 * 4];

using var c = XcsbClient.Connect();
var hasExt = c.Extensation.BigRequest();
if (hasExt is null || c.HandshakeSuccessResponseBody == null)
    return;

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


var req = data.AsSpan().Slice(0, 28);
var re = new PollyStruct(window, gc, WIDTH, HEIGHT, c.HandshakeSuccessResponseBody.Screens[0].RootDepth.DepthValue);
Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(req), re);

var body = data.AsSpan().Slice(28);
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

while (true)
{
    var evnt = d.GetEvent();
    if (evnt.ReplyType == Xcsb.Models.XEventType.LastEvent || evnt.Error.HasValue)
    { break; }

    if (evnt.ReplyType == Xcsb.Models.XEventType.Expose)
    {
        hasExt.BigRequestsEnable();
        c.SendRequest(data);
    }
}


[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 28)]
file struct PollyStruct(uint drawable, uint gc, ushort width, ushort height, byte depth)
{
    public readonly byte OpCode = 72;
    public readonly byte Format = 2;
    public readonly ushort Length = 0;
    public readonly uint Len = 250007;
    public readonly uint Drawable = drawable;
    public readonly uint Gc = gc;
    public readonly ushort Width = width;
    public readonly ushort Height = height;
    public readonly short X = 0;
    public readonly short Y = 0;
    public readonly byte LeftPad = 0;
    public readonly byte Depth = depth;
    private readonly ushort _pad = 0;
}