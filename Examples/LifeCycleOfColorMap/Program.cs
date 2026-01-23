using System.Diagnostics;
using Xcsb;
using Xcsb.Masks;
using Xcsb.Models;

using var connection = XcsbClient.Connect();
var x = XcsbClient.Initialized(connection);

var screen = connection.HandshakeSuccessResponseBody.Screens[0];
var root = screen.Root;

x.GrabServerChecked();

var colormap = connection.NewId();
x.CreateColormapChecked(Xcsb.Models.ColormapAlloc.None,
    colormap,
    root,
    screen.RootVisualId);

var win = connection.NewId();
x.CreateWindowChecked(screen.RootDepth!.DepthValue,
    win,
    root,
    0, 0, 500, 500,
    0, Xcsb.Models.ClassType.InputOutput, screen.RootVisualId,
    Xcsb.Masks.ValueMask.BackgroundPixel | Xcsb.Masks.ValueMask.EventMask | Xcsb.Masks.ValueMask.Colormap,
    [screen.WhitePixel, (uint)EventMask.ExposureMask, colormap]);

x.InstallColormapChecked(colormap);
Console.WriteLine("ColorMap installed.");

x.UngrabServerChecked();

x.MapWindowChecked(win);
Thread.Sleep(3000);

x.UninstallColormapChecked(colormap);
Console.WriteLine("ColorMap uninstalled.");

x.FreeColormapChecked(colormap);
Console.WriteLine("ColorMap freed.");

x.ConfigureWindowChecked(win, ConfigureValueMask.X | ConfigureValueMask.Y | ConfigureValueMask.Width | ConfigureValueMask.Height,
    [100, 100, 300, 300]);
Console.WriteLine("Window resized and moved to (100,100).");
Thread.Sleep(5000);
x.DestroyWindowChecked(win);


Thread.Sleep(1500);
win = connection.NewId();
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

var sub = connection.NewId();
x.CreateWindowChecked(0,
    sub, win,
    20, 20, 500, 250, 2,
    Xcsb.Models.ClassType.InputOutput, screen.RootVisualId, ValueMask.BackgroundPixel, [0xff0000]);
x.MapWindowChecked(sub);

Console.WriteLine("Subwindow created.");

Thread.Sleep(5000);

var sub1 = connection.NewId();
x.CreateWindowChecked(0,
    sub1, win,
    30, 30, 500, 250, 2,
    Xcsb.Models.ClassType.InputOutput, screen.RootVisualId, ValueMask.BackgroundPixel, [screen.WhitePixel]);
x.MapSubwindowsChecked(sub);
Console.WriteLine("Subwindow mapped.");

Thread.Sleep(5000);

x.ReparentWindowChecked(sub, sub1, 0, 0);
Thread.Sleep(millisecondsTimeout: 5000);


var colormap1 = connection.NewId();
x.CreateColormapChecked(Xcsb.Models.ColormapAlloc.None,
    colormap1,
    root,
    screen.RootVisualId);

var cmap = connection.NewId();
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
x.UngrabKeyboardUnchecked(0);

var xevnt = x.GetEvent();
Debug.Assert(xevnt.ReplyType == XEventType.Expose || xevnt.ReplyType == XEventType.MappingNotify);
// todo: fix this
Console.WriteLine("all success {0}", !xevnt.Error.HasValue);

x.UnmapSubwindowsChecked(sub);
x.DestroyWindowChecked(sub1);
x.DestroySubwindowsChecked(win);
x.DestroyWindowChecked(win);
Console.WriteLine("closing");
