using System.Diagnostics;
using Xcsb;
using Xcsb.Masks;
using Xcsb.Models.Event;

var x = XcsbClient.Initialized();

var screen = x.HandshakeSuccessResponseBody.Screens[0];
var root = screen.Root;

x.GrabServer();

var colormap = x.NewId();
x.CreateColormap(Xcsb.Models.ColormapAlloc.None,
    colormap,
    root,
    screen.RootVisualId);

var win = x.NewId();
x.CreateWindow(screen.RootDepth.DepthValue,
    win,
    root,
    0, 0, 500, 500,
    0, Xcsb.Models.ClassType.InputOutput, screen.RootVisualId,
    Xcsb.Masks.ValueMask.BackgroundPixel | Xcsb.Masks.ValueMask.EventMask | Xcsb.Masks.ValueMask.Colormap,
    [screen.WhitePixel, (uint)EventMask.ExposureMask, colormap]);

x.InstallColormap(colormap);
Console.WriteLine("Colormap installed.");

x.UngrabServer();

x.MapWindow(win);
Thread.Sleep(3000);

x.UninstallColormap(colormap);
Console.WriteLine("Colormap uninstalled.");

x.FreeColormap(colormap);
Console.WriteLine("Colormap freed.");

x.ConfigureWindow(win, ConfigureValueMask.X | ConfigureValueMask.Y | ConfigureValueMask.Width | ConfigureValueMask.Height,
    [100, 100, 300, 300]);
Console.WriteLine("Window resized and moved to (100,100).");
Thread.Sleep(5000);
x.DestroyWindow(win);


Thread.Sleep(1500);
win = x.NewId();
x.CreateWindow(screen.RootDepth.DepthValue,
    win,
    root,
    0, 0, 500, 500,
    0, Xcsb.Models.ClassType.InputOutput, screen.RootVisualId,
    Xcsb.Masks.ValueMask.BackgroundPixel | Xcsb.Masks.ValueMask.EventMask,
    [screen.WhitePixel, (uint)EventMask.ExposureMask]);
x.MapWindow(win);
Console.WriteLine("Reloading.");
Thread.Sleep(1500);

var sub = x.NewId();
x.CreateWindow(0,
    sub, win,
    20, 20, 50, 50, 2,
    Xcsb.Models.ClassType.InputOutput, screen.RootVisualId, ValueMask.BackgroundPixel, [0xff0000]);
x.MapWindow(sub);

Console.WriteLine("Subwindow created.");

Thread.Sleep(5000);

var sub1 = x.NewId();
x.CreateWindow(0,
    sub1, win,
    30, 30, 50, 50, 2,
    Xcsb.Models.ClassType.InputOutput, screen.RootVisualId, ValueMask.BackgroundPixel, [screen.WhitePixel]);
x.MapSubwindows(win);
Console.WriteLine("Subwindow mapped.");

Thread.Sleep(5000);

x.ReparentWindow(win, screen.Root, 200, 200);
Thread.Sleep(5000);



var cmap = x.NewId();
x.CopyColormapAndFree(cmap, screen.Root);
Console.WriteLine("Copied colormap and freed old one.");
Thread.Sleep(100);

x.FreeColormap(cmap);
Span<byte> evnt = stackalloc byte[XcsbClient.GetEventSize()];
ref var xevnt = ref x.GetEvent(evnt);
Debug.Assert(xevnt.EventType == EventType.Expose || xevnt.EventType == EventType.MappingNotify);
Console.WriteLine("all success {0}", xevnt.EventType != EventType.Error);
x.DestroySubwindows(win);
x.DestroyWindow(sub1);
x.DestroyWindow(win);
x.KillClient(win);
