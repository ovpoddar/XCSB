using Xcsb;
using Xcsb.Connection;
using Xcsb.Connection.Models.Handshake;

using var con = XcsbClient.Connect();
if (con.HandshakeSuccessResponseBody is null)
    return;
var screen = con.HandshakeSuccessResponseBody.Screens[0];
var visual = screen.Depths.FirstOrDefault(a => a.DepthValue == 32)?
    .Visuals.FirstOrDefault();
if (!visual.HasValue)
    return;

var client = con.Initialized();
var colormap = con.NewId();
client.CreateColormapChecked(Xcsb.Models.ColormapAlloc.None,
    colormap,
    screen.Root,
    visual.Value.VisualId);
var window = con.NewId();
client.CreateWindowChecked(
    32,
    window,
    screen.Root,
    0, 0, 300, 200, 0,
    Xcsb.Models.ClassType.InputOutput,
    visual.Value.VisualId,
    Xcsb.Masks.ValueMask.BackgroundPixel | Xcsb.Masks.ValueMask.BorderPixel | Xcsb.Masks.ValueMask.Colormap,
    [0x00000000, 0x00000000, colormap]
    );
client.MapWindowChecked(window);

var isRunning = true;
var gc = con.NewId();
client.CreateGCUnchecked(gc, window, Xcsb.Masks.GcMask.Foreground | Xcsb.Masks.GcMask.Background, [screen.WhitePixel, screen.BlackPixel]);
while (isRunning)
{
    var evnt = client.GetEvent();
    if (evnt.ReplyType == Xcsb.Models.XEventType.LastEvent)
    {
        isRunning = false;
        return;
    }

    if (evnt.Error.HasValue)
        isRunning = false;

    Console.WriteLine(evnt.ReplyType);
    client.PolyLineUnchecked(0, window, gc, [
        new(10, 10),
        new(1430, 10),
        new(1430, 868),
        new(10, 868),
        new(10, 10)
    ]);
}