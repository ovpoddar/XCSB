using Xcsb;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Event;
using Xcsb.Models.Handshake;

var x11 = XcsbClient.Initialized();
var rgbValue = x11.HandshakeSuccessResponseBody.Screens[0].Depths.FirstOrDefault(a => a.DepthValue == 32);
var visual = rgbValue?.Visuals.FirstOrDefault(a => a.Class == VisualClass.TrueColor && a.VisualId > 100 ).VisualId;

if (!visual.HasValue || rgbValue == null)
    return;

var root = x11.HandshakeSuccessResponseBody.Screens[0].Root;
var window = x11.NewId();
var white = x11.HandshakeSuccessResponseBody.Screens[0].WhitePixel;
var black = x11.HandshakeSuccessResponseBody.Screens[0].BlackPixel;
var cmid = x11.NewId();
x11.CreateColormap(ColormapAlloc.None, cmid, root, visual.Value);
x11.CreateWindowChecked(rgbValue.DepthValue, window, root, 10, 10, 168, 195, 1, ClassType.InputOutput,
    visual.Value, 
    ValueMask.EventMask | ValueMask.Colormap | ValueMask.BackgroundPixel | ValueMask.BorderPixel,
    [(uint)EventMask.ExposureMask, cmid, 0, 0]);
x11.MapWindow(window);

var isRunning = true;
var gc = x11.NewId();
x11.CreateGC(gc, window, GCMask.Foreground | GCMask.Background, [white, black]);
while (isRunning)
{
    var evnt = x11.GetEvent();
    if (!evnt.HasValue)
    {
        isRunning = false;
        return;
    }
    if (evnt.Value.EventType == EventType.Error)
        isRunning = false;
    
    Console.WriteLine(evnt.Value.EventType);
    x11.PolyLine(0, window, gc, [
        new(10, 10),
        new(1430, 10),
        new(1430, 868),
        new(10, 868),
        new(10, 10)
    ]);
}