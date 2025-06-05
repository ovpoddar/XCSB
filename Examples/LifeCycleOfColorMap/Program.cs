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
x.CreateWindow(win,
    root,
    0, 0, 500, 500,
    0, Xcsb.Models.ClassType.InputOutput, screen.RootVisualId,
    Xcsb.Masks.ValueMask.BackgroundPixel | Xcsb.Masks.ValueMask.EventMask | Xcsb.Masks.ValueMask.Colormap,
    [screen.WhitePixel, (uint)EventMask.ExposureMask, colormap]);

x.InstallColormap(colormap);
Console.WriteLine("Colormap installed.");
Thread.Sleep(3000);

x.UngrabServer();

x.MapWindow(win);

x.UninstallColormap(colormap);
Console.WriteLine("Colormap uninstalled.");

x.FreeColormap(colormap);
Console.WriteLine("Colormap freed.");

Span<byte> evnt = stackalloc byte[XcsbClient.GetEventSize()];
ref var xevnt = ref x.GetEvent(evnt);
Debug.Assert(xevnt.EventType == EventType.Expose);
Console.WriteLine("all success {0}", xevnt.EventType != EventType.Error);
x.DestroyWindow(win);