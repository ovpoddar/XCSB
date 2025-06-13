using System;
using System.Collections.Generic;
using System.Text;
using Xcsb;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Handshake;

int screen_num;
var x = XcsbClient.Initialized();

var screen = x.HandshakeSuccessResponseBody.Screens[0];
var root = screen.Root;

var win = x.NewId();
x.CreateWindow(screen.RootDepth.DepthValue,
    win,
    root,
    0, 0, 300, 200,
    1, Xcsb.Models.ClassType.InputOutput, screen.RootVisualId,
    0, []);
x.MapWindow(win);



var grabResult = x.GrabPointer(false,
    screen.Root,
    64,
    GrabMode.Asynchronous, GrabMode.Asynchronous,
    0, 0, 0);

Console.WriteLine($"Grab status {grabResult.Status}");

x.UngrabPointer(0);
Console.WriteLine("Ungrab pointer completed.");

var fontId = x.NewId();
x.OpenFont("fixed", fontId);

var _gc = x.NewId();
x.CreateGC(_gc, win, GCMask.Foreground | GCMask.Background | GCMask.Font, [screen.BlackPixel, screen.WhitePixel, fontId]);

x.ImageText8(win, _gc, 10, 40, "the background will change"u8);
Thread.Sleep(5000);
string[] colors = ["Red", "Green", "Blue"];
string[] propNames = ["COLOR_A", "COLOR_B", "COLOR_C"];
var atoms = new uint[3];

for (var i = 0; i < colors.Length; i++)
{
    var reply = x.InternAtom(false, propNames[i]);
    atoms[i] = reply.Atom;
    x.ChangeProperty<byte>(PropertyMode.Replace, win, atoms[i], 31, Encoding.UTF8.GetBytes(colors[i]));
}

for (var i = 0; i < 6; i++)
{
    var reply = x.GetProperty(false, win, atoms[0], 31, 0, 32);
    if (reply.Data.Length > 0)
    {
        x.ChangeWindowAttributes(win, ValueMask.BackgroundPixel, [(GetNameColor(reply.Data, screen))]);
        x.ClearArea(false, win, 0, 0, 0, 0);
    }

    x.RotateProperties(win, 1, atoms);
    Thread.Sleep(1000);
}

foreach (var atom in atoms)
    x.DeleteProperty(win, atom);

x.ImageText8(win, _gc, 10, 40, "Change the GC's foreground red to white"u8);
Thread.Sleep(5000);
var gc = x.NewId();
x.CreateGC(gc, win, GCMask.Foreground, [0xFF0000]);
var rect = new Rectangle()
{
    x = 10,
    y = 10,
    width = 100,
    height = 50,
};
x.PolyFillRectangle(win, gc, [rect]);

Thread.Sleep(1000);
x.ChangeGC(gc, GCMask.Foreground, [screen.WhitePixel]);
rect.x += 20;
rect.y += 60;

x.PolyFillRectangle(win, gc, [rect]);
x.FreeGC(gc);
Thread.Sleep(3000);

var gc1 = x.NewId();
var gc2 = x.NewId();
x.CreateGC(gc1, win, GCMask.Foreground, [0x0000FF]);
x.CreateGC(gc2, win, 0, []);
rect.x -= 15;
rect.y -= 65;
x.PolyFillRectangle(win, gc1, [rect]);
Thread.Sleep(1500);

x.CopyGC(gc1, gc2, GCMask.Foreground);
rect.x += 20;
rect.y += 60;

x.PolyFillRectangle(win, gc2, [rect]);

x.FreeGC(gc1);
Thread.Sleep(3000);

x.ChangePointerControl(new Acceleration { Denominator = 2, Numerator = 1 }, 4);
Console.WriteLine("Pointer control changed: acceleration 2:1, threshold 4 pixels");
Thread.Sleep(3000);

x.ChangeSaveSet(ChangeSaveSetMode.Insert, win);
Console.WriteLine("Window added to save set");
Thread.Sleep(3000);

x.ChangeSaveSet(ChangeSaveSetMode.Delete, win);
Console.WriteLine("Window removed from save set");
Thread.Sleep(3000);

x.ConvertSelection(win, 1, 31, 9, 0);
Console.WriteLine("Selection conversion requested");
Thread.Sleep(3000);


x.SetScreenSaver(5, 10, TriState.Yes, TriState.Yes);
Console.WriteLine("Screen saver set: 5s timeout, blanking preferred");
Thread.Sleep(20000);


x.ForceScreenSaver(ForceScreenSaverMode.Activate);
Console.WriteLine("Screen saver activated");
Thread.Sleep(3000);

x.ForceScreenSaver(ForceScreenSaverMode.Reset);
Console.WriteLine("Screen saver reset");
Thread.Sleep(3000);

x.NoOperation([1, 3, 323, 32, 2323]);
Console.WriteLine("NoOperation");
Thread.Sleep(3000);

x.SetAccessControl(AccessControlMode.Enable);
Console.WriteLine("Access control enabled");
Thread.Sleep(1500);

x.SetAccessControl(AccessControlMode.Disable);
Console.WriteLine("Access control disabled");
Thread.Sleep(1500);

x.StoreColors(screen.CMap, [new ColorItem
{
    Red = 32768,
    Green = 16384,
    Blue = 49152,
    ColorFlag = ColorFlag.Red | ColorFlag.Green | ColorFlag.Blue,
    Pixel = 1
}]);
Console.WriteLine("Color stored successfully at pixel.");

var font = x.NewId();

x.OpenFont("cursor", font);

var cursor = x.NewId();
x.CreateGlyphCursor(cursor, font, font, 'D', 69, // Arrow cursor
                       0, 0, 0, 65535, 65535, 65535);

Console.WriteLine("Created cursor with ID: {0}", cursor);
Thread.Sleep(3000);

x.FreeCursor(cursor);
Console.WriteLine("freed cursor");

x.CloseFont(font);



x.SetSelectionOwner(win, 1, 0);

Console.WriteLine("Selection owner set for PRIMARY selection");
Thread.Sleep(1500);

x.SetSelectionOwner(0, 1, 0);

Console.WriteLine("Selection owner cleared\n");


x.SetCloseDownMode(CloseDownMode.Destroy);

x.Dispose();
return 0;


uint GetNameColor(Span<byte> name, Screen screen)
{
    if (name.Slice(0, 3).SequenceEqual("Red"u8))
        return 0xFF0000;
    else if (name.Slice(0, 4).SequenceEqual("Blue"u8))
        return 0x0000FF;
    else if (name.Slice(0, 5).SequenceEqual("Green"u8))
        return 0x00FF00;

    return screen.WhitePixel;
}