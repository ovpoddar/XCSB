using Xcsb;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.ServerConnection.Handshake;
// todo:Reminder. this will work if their is the extension support
var x11 = XcsbClient.Initialized();
var rgbValue = x11.HandshakeSuccessResponseBody.Screens[0].Depths.FirstOrDefault(a => a.DepthValue == 32);
var visual = rgbValue?.Visuals.LastOrDefault(a => a.Class == VisualClass.TrueColor).VisualId;

if (!visual.HasValue || rgbValue == null)
    return;

var root = x11.HandshakeSuccessResponseBody.Screens[0].Root;
var window = x11.NewId();
var white = x11.HandshakeSuccessResponseBody.Screens[0].WhitePixel;
var black = x11.HandshakeSuccessResponseBody.Screens[0].BlackPixel;
var cmid = x11.NewId();
x11.CreateColormapUnchecked(ColormapAlloc.None, cmid, root, visual.Value);
x11.CreateWindowChecked(rgbValue.DepthValue, window, root, 10, 10, 168, 195, 1,
    ClassType.InputOutput, visual.Value,
    ValueMask.EventMask | ValueMask.BackgroundPixel | ValueMask.Colormap,
    [(uint)EventMask.ExposureMask, 0, cmid]);
x11.MapWindowUnchecked(window);

var isRunning = true;
var gc = x11.NewId();
x11.CreateGCUnchecked(gc, window, GCMask.Foreground | GCMask.Background, [white, black]);
while (isRunning)
{
    var evnt = x11.GetEvent();
    if (evnt.ReplyType == XEventType.LastEvent)
    {
        isRunning = false;
        return;
    }

    if (evnt.Error.HasValue)
        isRunning = false;

    Console.WriteLine(evnt.ReplyType);
    x11.PolyLineUnchecked(0, window, gc, [
        new(10, 10),
        new(1430, 10),
        new(1430, 868),
        new(10, 868),
        new(10, 10)
    ]);
}