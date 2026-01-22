using System.Diagnostics;
using Xcsb;
using Xcsb.Masks;
using Xcsb.Models;

var x = XcsbClient.Initialized();

var screen = x.HandshakeSuccessResponseBody.Screens[0];
var root = screen.Root;

x.CheckRequest( x.GrabServer());

var colormap = x.NewId();
x.CheckRequest(x.CreateColormap(Xcsb.Models.ColormapAlloc.None,
    colormap,
    root,
    screen.RootVisualId));

var win = x.NewId();
x.CheckRequest(x.CreateWindow(screen.RootDepth!.DepthValue,
    win,
    root,
    0, 0, 500, 500,
    0, Xcsb.Models.ClassType.InputOutput, screen.RootVisualId,
    Xcsb.Masks.ValueMask.BackgroundPixel | Xcsb.Masks.ValueMask.EventMask | Xcsb.Masks.ValueMask.Colormap,
    [screen.WhitePixel, (uint)EventMask.ExposureMask, colormap]));

x.CheckRequest(x.InstallColormap(colormap));
Console.WriteLine("ColorMap installed.");

x.CheckRequest(x.UngrabServer());

x.CheckRequest(x.MapWindow(win));
Thread.Sleep(3000);

x.CheckRequest(x.UninstallColormap(colormap));
Console.WriteLine("ColorMap uninstalled.");

x.CheckRequest(x.FreeColormap(colormap));
Console.WriteLine("ColorMap freed.");

x.CheckRequest(x.ConfigureWindow(win, ConfigureValueMask.X | ConfigureValueMask.Y | ConfigureValueMask.Width | ConfigureValueMask.Height,
    [100, 100, 300, 300]));
Console.WriteLine("Window resized and moved to (100,100).");
Thread.Sleep(5000);
x.CheckRequest(x.DestroyWindow(win));


Thread.Sleep(1500);
win = x.NewId();
x.CheckRequest(x.CreateWindow(screen.RootDepth.DepthValue,
    win,
    root,
    0, 0, 500, 500,
    0, Xcsb.Models.ClassType.InputOutput, screen.RootVisualId,
    Xcsb.Masks.ValueMask.BackgroundPixel | Xcsb.Masks.ValueMask.EventMask,
    [screen.WhitePixel, (uint)EventMask.ExposureMask]));
x.CheckRequest(x.MapWindow(win));
Console.WriteLine("Reloading.");
Thread.Sleep(1500);

var sub = x.NewId();
x.CheckRequest(x.CreateWindow(0,
    sub, win,
    20, 20, 500, 250, 2,
    Xcsb.Models.ClassType.InputOutput, screen.RootVisualId, ValueMask.BackgroundPixel, [0xff0000]));
x.CheckRequest(x.MapWindow(sub));

Console.WriteLine("Subwindow created.");

Thread.Sleep(5000);

var sub1 = x.NewId();
x.CheckRequest(x.CreateWindow(0,
    sub1, win,
    30, 30, 500, 250, 2,
    Xcsb.Models.ClassType.InputOutput, screen.RootVisualId, ValueMask.BackgroundPixel, [screen.WhitePixel]));
x.CheckRequest(x.MapSubwindows(sub));
Console.WriteLine("Subwindow mapped.");

Thread.Sleep(5000);

x.CheckRequest(x.ReparentWindow(sub, sub1, 0, 0));
Thread.Sleep(millisecondsTimeout: 5000);


var colormap1 = x.NewId();
x.CheckRequest(x.CreateColormap(Xcsb.Models.ColormapAlloc.None,
    colormap1,
    root,
    screen.RootVisualId));

var cmap = x.NewId();
x.CheckRequest(x.CopyColormapAndFree(cmap, colormap1));
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
x.UngrabKeyboardUnchecked(0);

var xevnt = x.GetEvent();
Debug.Assert(xevnt.ReplyType == XEventType.Expose || xevnt.ReplyType == XEventType.MappingNotify);
// todo: fix this
Console.WriteLine("all success {0}", !xevnt.Error.HasValue);

x.CheckRequest(x.UnmapSubwindows(sub));
x.CheckRequest(x.DestroyWindow(sub1));
x.CheckRequest(x.DestroySubwindows(win));
x.CheckRequest(x.DestroyWindow(win));
Console.WriteLine("closing");
