using System.Diagnostics;
using Xcsb;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Event;

var x = XcsbClient.Initialized();

var screen = x.HandshakeSuccessResponseBody.Screens[0];
var root = screen.Root;

x.GrabServerChecked();

var colormap = x.NewId();
x.CreateColormapChecked(Xcsb.Models.ColormapAlloc.None,
    colormap,
    root,
    screen.RootVisualId);

var win = x.NewId();
x.CreateWindowChecked(screen.RootDepth.DepthValue,
    win,
    root,
    0, 0, 500, 500,
    0, Xcsb.Models.ClassType.InputOutput, screen.RootVisualId,
    Xcsb.Masks.ValueMask.BackgroundPixel | Xcsb.Masks.ValueMask.EventMask | Xcsb.Masks.ValueMask.Colormap,
    [screen.WhitePixel, (uint)EventMask.ExposureMask, colormap]);

x.InstallColormapChecked(colormap);
Console.WriteLine("Colormap installed.");

x.UngrabServerChecked();

x.MapWindowChecked(win);
Thread.Sleep(3000);

x.UninstallColormapChecked(colormap);
Console.WriteLine("Colormap uninstalled.");

x.FreeColormapChecked(colormap);
Console.WriteLine("Colormap freed.");

x.ConfigureWindowChecked(win, ConfigureValueMask.X | ConfigureValueMask.Y | ConfigureValueMask.Width | ConfigureValueMask.Height,
    [100, 100, 300, 300]);
Console.WriteLine("Window resized and moved to (100,100).");
Thread.Sleep(5000);
x.DestroyWindowChecked(win);


Thread.Sleep(1500);
win = x.NewId();
x.CreateWindowChecked(screen.RootDepth.DepthValue,
    win,
    root,
    0, 0, 500, 500,
    0, Xcsb.Models.ClassType.InputOutput, screen.RootVisualId,
    Xcsb.Masks.ValueMask.BackgroundPixel | Xcsb.Masks.ValueMask.EventMask,
    [screen.WhitePixel, (uint)EventMask.ExposureMask]);
x.MapWindowChecked(win);
Console.WriteLine("Reloading.");
Thread.Sleep(1500);

var sub = x.NewId();
x.CreateWindowChecked(0,
    sub, win,
    20, 20, 500, 250, 2,
    Xcsb.Models.ClassType.InputOutput, screen.RootVisualId, ValueMask.BackgroundPixel, [0xff0000]);
x.MapWindowChecked(sub);

Console.WriteLine("Subwindow created.");

Thread.Sleep(5000);

var sub1 = x.NewId();
x.CreateWindowChecked(0,
    sub1, win,
    30, 30, 500, 250, 2,
    Xcsb.Models.ClassType.InputOutput, screen.RootVisualId, ValueMask.BackgroundPixel, [screen.WhitePixel]);
x.MapSubwindowsChecked(win);
Console.WriteLine("Subwindow mapped.");

Thread.Sleep(5000);

x.ReparentWindowChecked(sub, sub1, 0, 0);
Thread.Sleep(millisecondsTimeout: 5000);


var colormap1 = x.NewId();
x.CreateColormapChecked(Xcsb.Models.ColormapAlloc.None,
    colormap1,
    root,
    screen.RootVisualId);

var cmap = x.NewId();
x.CopyColormapAndFreeChecked(cmap, colormap1);
Console.WriteLine("Copied colormap and freed old one.");
Thread.Sleep(100);


var resultGrabKeyboard = x.GrabKeyboard(
    false,
    win,
    0,
    GrabMode.Asynchronous,
    GrabMode.Asynchronous
);
        
Console.WriteLine($"grabbing all keys for this window {resultGrabKeyboard.Status}");
x.UngrabKeyboard(0);

var xevnt = x.GetEvent();
Debug.Assert(xevnt!.Value.EventType == EventType.Expose || xevnt!.Value.EventType == EventType.MappingNotify);
Console.WriteLine("all success {0}", xevnt!.Value.EventType != EventType.Error);

x.DestroyWindowChecked(sub);
Console.WriteLine("closing");
x.DestroyWindowChecked(sub1);
x.DestroySubwindowsChecked(win);
x.DestroyWindowChecked(win);
