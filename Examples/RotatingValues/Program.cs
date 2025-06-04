using System;
using System.Collections.Generic;
using System.Text;
using Xcsb;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Handshake;

int screen_num;
var x = XcsbClient.Initialized();

// Get screen and root
var screen = x.HandshakeSuccessResponseBody.Screens[0];
var root = screen.Root;

// Create a basic window
var win = x.NewId();
x.CreateWindow(
    win,
    root,
    0, 0, 300, 200,
    1, Xcsb.Models.ClassType.InputOutput, screen.RootVisualId,
    0, []);
x.MapWindow(win);

// Create three properties

var fontId = x.NewId();
x.OpenFont("fixed", fontId);
var _gc = x.NewId();
x.CreateGC(_gc, win, GCMask.Foreground | GCMask.Background | GCMask.Font, [screen.BlackPixel, screen.WhitePixel, fontId]);

x.ImageText8(win, _gc, 10, 40, "the background will change"u8);
Thread.Sleep(5000);
string[] colors = ["Red", "Green", "Blue"];
string[] propNames = ["COLOR_A", "COLOR_B", "COLOR_C"];
var atoms = new uint[3];

for (int i = 0; i < colors.Length; i++)
{
    var reply = x.InternAtom(false, propNames[i]);
    atoms[i] = reply.Atom;
    x.ChangeProperty<byte>(PropertyMode.Replace, win, atoms[i], 31, Encoding.UTF8.GetBytes(colors[i]));
}

for (int i = 0; i < 6; i++)
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

Thread.Sleep(3000);

x.Dispose();
return 0;


uint GetNameColor(Span<byte> name, Screen screen)
{
    if (name.Slice(0, 3).SequenceEqual("Red"u8))
        return 0xFF0000;
    if (name.Slice(0, 4).SequenceEqual("Blue"u8))
        return 0x0000FF;
    if (name.Slice(0, 5).SequenceEqual("Green"u8))
        return 0x00FF00;
    
    return screen.WhitePixel;
}